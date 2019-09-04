using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoFilesLibrary.Helpers {
    public  abstract class Constants {
        public const string vStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=filestoragevideolibrary;AccountKey=V3zEEbkNbLWfu0nXoxVEDU5zVvb64PQ3QnGOwuuUq1qxQyuiplVrfGddm6QZa4e1kGuNy+M2guV/3jWSOEl9mw==;EndpointSuffix=core.windows.net";
        public const string vContainerDefault = "videos";
        public const string vTextFileFolder = "Please Not Delete This File! Your App May Malfunction";
        public const string vIconFolder = "fas fa-folder";
        public const string vIconFile = "far fa-file-video";
    }
}
