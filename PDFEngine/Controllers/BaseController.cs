using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PDFEngine.Models;

namespace PDFEngine.Controllers
{
    public class BaseController : Controller
    {
        protected Entities1 DataStore;

        public BaseController()
        {
            DataStore = new Entities1();
            DataStore.Configuration.ProxyCreationEnabled = false;
            DataStore.Configuration.LazyLoadingEnabled = false;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (DataStore != null)
            {
                DataStore.Dispose();
            }
        }
    }
}
