using System;
using System.Text.RegularExpressions;

namespace SikuliSharp
{
    public interface ISikuliSession : IDisposable
    {
        bool Exists(IPattern pattern, float timeoutInSeconds = 0, IPattern parent = null);
        bool Click(IPattern pattern, IPattern parent = null);
        bool Click(IPattern pattern, Point offset, IPattern parent = null);
        bool DoubleClick(IPattern pattern, IPattern parent = null);
        bool DoubleClick(IPattern pattern, Point offset, IPattern parent = null);
        bool Wait(IPattern pattern, float timeoutInSeconds = 0, IPattern parent = null);
        bool WaitVanish(IPattern pattern, float timeoutInSeconds = 0, IPattern parent = null);
        bool Type(string text);
        bool Type(IPattern pattern, string text, IPattern parent = null);
        bool Hover(IPattern pattern, IPattern parent = null);
        bool Hover(IPattern pattern, Point offset, IPattern parent = null);
        bool RightClick(IPattern pattern, IPattern parent = null);
        bool RightClick(IPattern pattern, Point offset, IPattern parent = null);
        bool DragDrop(IPattern fromPattern, IPattern toPattern);
        bool Type(IPattern pattern, string text, Point offset, IPattern parent = null);
        string FindXCordinate(IPattern pattern, float commandParameter);
        string FindYCordinate(IPattern pattern, float commandParameter);    
        string FindWCordinate(IPattern pattern, float commandParameter);
        string FindHCordinate(IPattern pattern, float commandParameter);
        string GetTextOfRegion(int x, int y,int width,int height);
        string FindCenterCordinate(IPattern pattern, float commandParameter);
    }

    public class SikuliSession : ISikuliSession
    {
        private static readonly Regex InvalidTextRegex = new Regex(@"[\r\n\t\x00-\x1F]", RegexOptions.Compiled);
        private readonly ISikuliRuntime _runtime;

        public SikuliSession(ISikuliRuntime sikuliRuntime)
        {
            _runtime = sikuliRuntime;
            _runtime.Start();
        }

       

        public bool Exists(IPattern pattern, float timeoutInSeconds = 0, IPattern parent = null)
        {
            if (parent != null)
                return RunCommand("exists", pattern, timeoutInSeconds, parent);
            else
                return RunCommand("exists", pattern, timeoutInSeconds);
        }

        public bool Click(IPattern pattern, IPattern parent = null)
        {
            if (parent != null)
                return RunCommand("click", pattern, 0, parent);
            else
                return RunCommand("click", pattern, 0);
        }

        public bool Click(IPattern pattern, Point offset, IPattern parent = null)
        {
            if (parent != null)
                return RunCommand("click", new WithOffsetPattern(pattern, offset), 0, parent);
            else
                return RunCommand("click", new WithOffsetPattern(pattern, offset), 0);
        }

        public bool DoubleClick(IPattern pattern, IPattern parent = null)
        {
            if (parent != null)
                return RunCommand("doubleClick", pattern, 0, parent);
            else
                return RunCommand("doubleClick", pattern, 0);
        }

        public bool DoubleClick(IPattern pattern, Point offset, IPattern parent = null)
        {
            if (parent != null)
                return RunCommand("doubleClick", new WithOffsetPattern(pattern, offset), 0, parent);
            else
                return RunCommand("doubleClick", new WithOffsetPattern(pattern, offset), 0);
        }

        public bool Wait(IPattern pattern, float timeoutInSeconds = 0f, IPattern parent = null)
        {
            if (parent != null)
                return RunCommand("wait", pattern, timeoutInSeconds, parent);
            else
                return RunCommand("wait", pattern, timeoutInSeconds);
        }

        public bool WaitVanish(IPattern pattern, float timeoutInSeconds = 0f, IPattern parent = null)
        {
            if (parent != null)
                return RunCommand("waitVanish", pattern, timeoutInSeconds, parent);
            else
                return RunCommand("waitVanish", pattern, timeoutInSeconds);
        }

        public bool Type(string text)
        {
            if (InvalidTextRegex.IsMatch(text))
                throw new ArgumentException("Text cannot contain control characters. Escape them before, e.g. \\n should be \\\\n", "text");

            var script = string.Format(
                "print \"SIKULI#: YES\" if type(\"{0}\") == 1 else \"SIKULI#: NO\"",
                text
                );

            var result = _runtime.Run(script, "SIKULI#: ", 0d);
            return result.Contains("SIKULI#: YES");
        }
        public bool Type(IPattern pattern, string text, IPattern parent = null)
        {
            if (parent != null)
                return RunCommand("type", pattern, text, 0, parent);
            else
                return RunCommand("type", pattern, text, 0);
        }
        public bool Type(IPattern pattern, string text, Point offset, IPattern parent = null)
        {
            if (parent != null)
                return RunCommand("type", new WithOffsetPattern(pattern, offset), text, 0, parent);
            else
                return RunCommand("type", new WithOffsetPattern(pattern, offset), text, 0);
        }

        public bool Hover(IPattern pattern, IPattern parent = null)
        {

            if (parent != null)
                return RunCommand("hover", pattern, 0, parent);
            else
                return RunCommand("hover", pattern, 0);
        }

        public bool Hover(IPattern pattern, Point offset, IPattern parent = null)
        {
            if (parent != null)
                return RunCommand("hover", new WithOffsetPattern(pattern, offset), 0, parent);
            else
                return RunCommand("hover", new WithOffsetPattern(pattern, offset), 0);
        }

        public bool RightClick(IPattern pattern, IPattern parent = null)
        {

            if (parent != null)
                return RunCommand("rightClick", pattern, 0, parent);
            else
            if (parent != null)
                return RunCommand("rightClick", pattern, 0, parent);
            else
                return RunCommand("rightClick", pattern, 0);
        }

        public bool RightClick(IPattern pattern, Point offset, IPattern parent = null)
        {
            if (parent != null)
                return RunCommand("rightClick", new WithOffsetPattern(pattern, offset), 0, parent);
            else
                return RunCommand("rightClick", new WithOffsetPattern(pattern, offset), 0);
        }

        public bool DragDrop(IPattern fromPattern, IPattern toPattern)
        {
            return RunCommand("dragDrop", fromPattern, toPattern, 0);
        }

        protected bool RunCommand(string command, IPattern pattern, float commandParameter, IPattern parent)
        {
            pattern.Validate();

            var script = string.Format(
                "print \"SIKULI#: YES\" if find({0}).{1}({2}{3}) else \"SIKULI#: NO\"",
                parent.ToSikuliScript(),
                command,
                pattern.ToSikuliScript(),
                ToSukuliFloat(commandParameter)
                );

            var result = _runtime.Run(script, "SIKULI#: ", commandParameter * 1.5d); // Failsafe
            return result.Contains("SIKULI#: YES");
        }

        protected bool RunCommand(string command, IPattern pattern, float commandParameter)
        {
            pattern.Validate();

            var script = string.Format(
                "print \"SIKULI#: YES\" if {0}({1}{2}) else \"SIKULI#: NO\"",
                command,
                pattern.ToSikuliScript(),
                ToSukuliFloat(commandParameter)
                );

            var result = _runtime.Run(script, "SIKULI#: ", commandParameter * 1.5d); // Failsafe
			
            return result.Contains("SIKULI#: YES");
        }
        public string FindXCordinate(IPattern pattern, float commandParameter)
        {

            pattern.Validate();

            var script = string.Format(
                "print find({0}).getX()",
                //command,
                pattern.ToSikuliScript()
                //ToSukuliFloat(commandParameter)
                );

            var result = _runtime.Run(script, "", commandParameter * 1.5d); // Failsafe
            return result;
        }
        public string FindYCordinate(IPattern pattern, float commandParameter)
        {

            pattern.Validate();

            var script = string.Format(
               "print find({0}).getY()",
                //command,
                pattern.ToSikuliScript()
                //ToSukuliFloat(commandParameter)
                );

            var result = _runtime.Run(script, "", commandParameter * 1.5d); // Failsafe
            return result;
        }

        public string FindWCordinate(IPattern pattern, float commandParameter)
        {

            pattern.Validate();

            var script = string.Format(
                "print find({0}).getW()",
                //command,
                pattern.ToSikuliScript()
                //ToSukuliFloat(commandParameter)
                );

            var result = _runtime.Run(script, "", commandParameter * 1.5d); // Failsafe
            return result;
        }
        public string FindHCordinate(IPattern pattern, float commandParameter)
        {

            pattern.Validate();

            var script = string.Format(
                "print find({0}).getH()",
                //command,
                pattern.ToSikuliScript()
                //ToSukuliFloat(commandParameter)
                );

            var result = _runtime.Run(script, "", commandParameter * 1.5d); // Failsafe
            return result;
        }

        public string FindCenterCordinate(IPattern pattern, float commandParameter)
        {

            pattern.Validate();

            var script = string.Format(
                "print find({0}).getCenter()",
                //command,
                pattern.ToSikuliScript()
                //ToSukuliFloat(commandParameter)
                );

            var result = _runtime.Run(script, "", commandParameter * 1.5d); // Failsafe
            return result;
        }
        protected bool RunCommand(string command, IPattern pattern, string text, float input)
        {
            pattern.Validate();

            var script = string.Format(
                "print \"SIKULI#: YES\" if {0}({1},\"{2}\",{3}) else \"SIKULI#: NO\"",
                command,
                pattern.ToSikuliScript(), text, input
                );

            var result = _runtime.Run(script, "SIKULI#: ", 0d); // Failsafe
            return result.Contains("SIKULI#: YES");
        }

        protected bool RunCommand(string command, IPattern pattern, string text, float input, IPattern parent)
        {
            pattern.Validate();

            var script = string.Format(
                "print \"SIKULI#: YES\" if find({0}).{1}({2},\"{3}\",{4}) else \"SIKULI#: NO\"",
                parent.ToSikuliScript(), command, text, input,
                pattern.ToSikuliScript()
                );

            var result = _runtime.Run(script, "SIKULI#: ", 0d); // Failsafe
            return result.Contains("SIKULI#: YES");
        }

        public string GetTextOfRegion(int x, int y,int width,int height)
        {
            var script = string.Format(
                "print Region({0},{1},{2},{3}).text()",
                x, y, width, height
                );

            var result = _runtime.Run(script, "", 0d); // Failsafe
            return result;
        }

        protected bool RunCommand(string command, IPattern fromPattern, IPattern toPattern, float commandParameter)
        {
            fromPattern.Validate();
            toPattern.Validate();

            var script = string.Format(
                "print \"SIKULI#: YES\" if {0}({1},{2}{3}) else \"SIKULI#: NO\"",
                command,
                fromPattern.ToSikuliScript(),
                toPattern.ToSikuliScript(),
                ToSukuliFloat(commandParameter)
                );

            var result = _runtime.Run(script, "SIKULI#: ", commandParameter * 1.5d); // Failsafe
            return result.Contains("SIKULI#: YES");
        }



        private static string ToSukuliFloat(float timeoutInSeconds)
        {
            return timeoutInSeconds > 0f ? ", " + timeoutInSeconds.ToString("0.####") : "";
        }

        public void Dispose()
        {
            _runtime.Stop();
        }
    }
}