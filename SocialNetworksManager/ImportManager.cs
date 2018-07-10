using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using SocialNetworksManager.Contracts;

namespace SocialNetworksManager
{
    //Класс для отлавливания событий
    class ImportEventArgs : EventArgs
    {
        private String status;

        public String Status
        {
            get { return status; }
            set { this.status = value; }
        }

        public ImportEventArgs(String status)
        {
            this.status = status;
        }
    }

    //Класс для импорта расширений
    class ImportManager : IPartImportsSatisfiedNotification
    {
        public event EventHandler<ImportEventArgs> ImportSatisfied;

        [ImportMany(typeof(ISocialNetworksManagerExtension), AllowRecomposition = true)]
        public IEnumerable<Lazy<ISocialNetworksManagerExtension>> extensionsCollection { get; set; }

        public void OnImportsSatisfied()
        {
            if(ImportSatisfied != null)
            {
                ImportSatisfied.Invoke(this, new ImportEventArgs("Import Satisfied."));
            }
        }
    }
}
