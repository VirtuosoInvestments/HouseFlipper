﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.HouseFlipper
{
    public abstract class View
    {
        private MainSite site;
        public View(MainSite mainSite)
        {
            this.site = mainSite;
        }

        public virtual void GoToView()
        {
            site.GoToSite();
            HandleNavigateToView();
        }

        protected abstract void HandleNavigateToView();
    }
}
