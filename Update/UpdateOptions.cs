namespace FlexibleDBMS
{
    public class UpdateOptions 
    {
        public string pathToUpdateZip { get; set; }
        public string appUpdateMD5 { get; set; }
        public string pathToXml { get; set; }
        public string appVersion { get; set; }
        public string serverUpdateURI { get; set; }
        public string localAppFolderPath { get; set; }
        public string localUpdateFolderPath { get; set; }
        public string appUpdateURL { get; set; }
        public string appUpdateFolderURI { get; set; }
        public string appUpdateChangeLogURL { get; set; }

        public UpdateOptions() { }

        public UpdateOptions(UpdateOptions parameters)
        { SetUpdatingParameters(parameters); }

        private void SetUpdatingParameters(UpdateOptions parameters)
        {
            serverUpdateURI = parameters?.serverUpdateURI;
            localAppFolderPath = parameters?.localAppFolderPath;
            localUpdateFolderPath = parameters?.localUpdateFolderPath;
            appUpdateFolderURI = parameters?.appUpdateFolderURI;
            appUpdateURL = parameters?.appUpdateURL;
            pathToXml = parameters?.pathToXml;
            appUpdateChangeLogURL = parameters?.appUpdateChangeLogURL;
            appUpdateMD5 = parameters?.appUpdateMD5;
            appVersion = parameters?.appVersion;
            pathToUpdateZip = parameters?.pathToUpdateZip;
        }

        public UpdateOptions Get()
        { return new UpdateOptions(this); }
    }
}
