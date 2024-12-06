using Advent2024.Day06.Core;

namespace Advent2024.Day06.Win
{
    internal class WindowRenderDevice : IRenderDevice
    {
        private Form1 form1;
        private PictureBox pictureBox;

        public WindowRenderDevice(Form1 form1, PictureBox pictureBox)
        {
            this.form1 = form1;
            this.pictureBox = pictureBox;
        }

        public void BeginFrame()
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public char ReadKey()
        {
            throw new NotImplementedException();
        }

        public void WriteGlyph(char c)
        {
            throw new NotImplementedException();
        }

        public void WriteLine()
        {
            throw new NotImplementedException();
        }

        public void WriteStatusLine(string v)
        {
            throw new NotImplementedException();
        }
    }
}