using System;

namespace SlackExtension.Models
{
    public static class SlackFileTypes
    {
        private static String _all = "all";
        private static String _spaces = "spaces";
        private static String _snippets = "snippets";
        private static String _images = "images";
        private static String _gdocs = "gdocs";
        private static String _zips = "zips";
        private static String _pdfs = "pdfs";

        public static SlackFileType All { get { return new SlackFileType(_all); } }
        public static SlackFileType Spaces { get { return new SlackFileType(_spaces); } }
        public static SlackFileType Snippets { get { return new SlackFileType(_snippets); } }
        public static SlackFileType Images { get { return new SlackFileType(_images); } }
        public static SlackFileType GDocs { get { return new SlackFileType(_gdocs); } }
        public static SlackFileType Zips { get { return new SlackFileType(_zips); } }
        public static SlackFileType PDFs { get { return new SlackFileType(_pdfs); } }
    }

    public class SlackFileType
    {
        private String _filetypebase;

        public SlackFileType(String filetype)
        {
            _filetypebase = filetype;
        }

        public override String ToString()
        {
            return _filetypebase;
        }

        public static SlackFileType operator | (SlackFileType a,SlackFileType b)
        {
            return new SlackFileType(a._filetypebase+","+b._filetypebase);
        }
    }
}