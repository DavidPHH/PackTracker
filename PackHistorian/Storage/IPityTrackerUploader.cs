using PackTracker.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PackTracker.Storage {
    internal interface IPityTrackerUploader {
        void UploadPack(string Cookie, string AuthToken, Pack Pack);
    }
}
