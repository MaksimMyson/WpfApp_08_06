
namespace System.Windows
{
    internal class Forms
    {
        public static object DialogResult { get; internal set; }

        internal class FolderBrowserDialog
        {
            public FolderBrowserDialog()
            {
            }

            public string SelectedPath { get; internal set; }

            internal object ShowDialog()
            {
                throw new NotImplementedException();
            }
        }
    }
}