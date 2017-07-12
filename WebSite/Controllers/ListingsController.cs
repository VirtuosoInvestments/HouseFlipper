using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HouseFlipper.DataAccess.DB;
using HouseFlipper.DataAccess.Models;
using PagedList;
using System.Reflection;
using Google.Maps.Geocoding;
using HouseFlipper.DataAccess;

namespace HouseFlipper.WebSite.Controllers
{    
    public class ListingsController : Controller
    {
        private MlsContext db = new MlsContext();
        //private SortDirection sortDirection = SortDirection.None;

        //[HttpGet]
        //public ActionResult AllListings()
        //{
        //    return View(db.Listings);
        //}

        //[HttpGet]
        //public ActionResult AllListings(int page = 1, int pageSize = 10)
        //{
        //    List<MlsRow> listings = db.Listings.OrderBy(x => x.MLNumber).ToList();
        //    PagedList<MlsRow> model = new PagedList<MlsRow>(listings, page, pageSize);
        //    return View(model);
        //}

        // GET: Listings
        //public ActionResult Index()
        //{
        //    return View(db.Listings.Take(5).ToList());
        //}

        public List<Listing> GetActive()
        {
            var listings = from l in db.Listings
                           //where l.StatusValue() == MlsStatus.Active
                           where l.Status.ToLower().Trim() == "act"
                           select l;
            return listings.ToList();
        }

        public ActionResult Index(string sortOrder, string CurrentSort, string currentFilter, string searchString, SortDirection? sortDirection, int? page)
        {

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            GeoLocationController geoLocCtrl = new GeoLocationController();
            var listings = from l in db.Listings
                           select l;
            if (!String.IsNullOrEmpty(searchString))
            {
                var search = searchString.ToUpper();
                listings = listings.Where(
                    s => s.MLNumber.ToUpper().Contains(search) ||
                         s.Status.ToUpper().Contains(search) ||
                         s.Address.ToUpper().Contains(search) ||
                         s.City.ToUpper().Contains(search) ||
                         s.LegalSubdivisionName.ToUpper().Contains(search) ||
                         s.PostalCode.ToUpper().Contains(search) ||
                         s.CurrentPrice.ToUpper().Contains(search) ||
                         s.SqFtHeated.ToUpper().Contains(search) ||
                         s.LPSqFt.ToUpper().Contains(search) ||
                         s.SPSqFt.ToUpper().Contains(search) ||
                         s.Beds.ToUpper().Contains(search) ||
                         s.FullBaths.ToUpper().Contains(search) ||
                         s.YearBuilt.ToUpper().Contains(search) ||
                         s.Pool.ToUpper().Contains(search) ||
                         s.Taxes.ToUpper().Contains(search) ||
                         s.CloseDate.ToUpper().Contains(search));
            }
            /*
            bool latLongUpdated = false;
            listings.ToList().ForEach(
                (row) =>
                {
                    if(!row.Latitude.HasValue)
                    //if(row.Latitude==0 && row.Longitude==0)
                    {                        
                        var loc = geoLocCtrl.GetLatLong(row.Address + ", " + row.City + ", FL " + " " + row.PostalCode);
                        if (loc != null)
                        {
                            latLongUpdated = true;
                            row.Latitude = loc.Latitude;
                            row.Longitude = loc.Longitude;
                        }
                    }
                }
                );
            if(latLongUpdated)
            {
                db.SaveChanges();
            }*/

            int pageSize = 500;
            int pageIndex = 1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;


                                
            sortOrder = String.IsNullOrEmpty(sortOrder) ? "MLNumber" : sortOrder;
            ViewBag.CurrentSort = sortOrder;    

            IPagedList<Listing> pageList = null;

            switch (sortOrder)
            {
                case "MLNumber":
                    SetSortDirection(sortOrder, CurrentSort, sortDirection, page);
                    if (ViewBag.SortDirection == SortDirection.Descending)
                    {
                        pageList = listings.OrderByDescending
                                (m => m.MLNumber).ToPagedList(pageIndex, pageSize);
                    }
                    else
                    {
                        pageList = listings.OrderBy
                                    (m => m.MLNumber).ToPagedList(pageIndex, pageSize);
                    }
                            
                    break;
                case "Status":
                    SetSortDirection(sortOrder, CurrentSort, sortDirection, page);
                    if (ViewBag.SortDirection == SortDirection.Descending)                    
                    {
                        pageList = listings.OrderByDescending
                                (m => m.Status).ToPagedList(pageIndex, pageSize);
                        ViewBag.SortDirection = SortDirection.Descending;
                    }
                    else
                    {
                        pageList = listings.OrderBy
                                (m => m.Status).ToPagedList(pageIndex, pageSize);
                        ViewBag.SortDirection = SortDirection.Ascending;
                    }
                    break;

                case "Address":
                    SetSortDirection(sortOrder, CurrentSort, sortDirection, page);
                    if (ViewBag.SortDirection == SortDirection.Descending)
                    {
                        pageList = listings.OrderByDescending
                                (m => m.Address).ToPagedList(pageIndex, pageSize);
                        ViewBag.SortDirection = SortDirection.Descending;
                    }
                    else
                    {
                        pageList = listings.OrderBy
                                (m => m.Address).ToPagedList(pageIndex, pageSize);
                        ViewBag.SortDirection = SortDirection.Ascending;
                    }
                    break;

                case "City":
                    SetSortDirection(sortOrder, CurrentSort, sortDirection, page);
                    if (ViewBag.SortDirection == SortDirection.Descending)
                    {
                        pageList = listings.OrderByDescending
                                (m => m.City).ToPagedList(pageIndex, pageSize);
                        ViewBag.SortDirection = SortDirection.Descending;
                    }
                    else
                    {
                        pageList = listings.OrderBy
                                (m => m.City).ToPagedList(pageIndex, pageSize);
                        ViewBag.SortDirection = SortDirection.Ascending;
                    }
                    break;

                case "CurrentPrice":
                    SetSortDirection(sortOrder, CurrentSort, sortDirection, page);
                    if (ViewBag.SortDirection == SortDirection.Descending)
                    {
                        pageList = listings.ToList().OrderByDescending
                                (m => m.CurrentPriceValue(), new NumericComparer(sortOrder, SortDirection.Descending)).ToPagedList(pageIndex, pageSize);
                        ViewBag.SortDirection = SortDirection.Descending;
                    }
                    else
                    {
                        pageList = listings.ToList().OrderBy
                                (m => m.CurrentPriceValue(), new NumericComparer(sortOrder, SortDirection.Ascending)).ToPagedList(pageIndex, pageSize);
                        ViewBag.SortDirection = SortDirection.Ascending;
                    }
                    break;

                default:
                    SetSortDirection(sortOrder, CurrentSort, sortDirection, page);
                    if (ViewBag.SortDirection == SortDirection.Descending)
                    {
                        pageList = listings.ToList().OrderByDescending
                                (m => m, new MlsRowComparer(sortOrder)).ToPagedList(pageIndex, pageSize);
                        ViewBag.SortDirection = SortDirection.Descending;
                    }
                    else
                    {
                        pageList = listings.ToList().OrderBy
                                (m => m, new MlsRowComparer(sortOrder)).ToPagedList(pageIndex, pageSize);
                        ViewBag.SortDirection = SortDirection.Ascending;
                    }
                    break;

                // Add sorting statements for other columns

                //case "Default":
                //    products = listings.OrderBy
                //            (m => m.MLNumber).ToPagedList(pageIndex, pageSize);
                //    sortDirection = SortDirection.Ascending;
                //    break;

                //default:
                //    SetSortDirection(sortOrder, CurrentSort, sortDirection, page);
                //    if (ViewBag.SortDirection == SortDirection.Descending)
                //    {
                //        products = listings.ToList().OrderByDescending
                //                (m => GetPropertyValue(m,sortOrder)).ToPagedList(pageIndex, pageSize);
                //        ViewBag.SortDirection = SortDirection.Descending;
                //    }
                //    else
                //    {
                //        products = listings.ToList().OrderBy
                //                (m => GetPropertyValue(m, sortOrder)).ToPagedList(pageIndex, pageSize);
                //        ViewBag.SortDirection = SortDirection.Ascending;
                //    }
                //    break;
            }
            return View(pageList);
        }

        private void SetSortDirection(string newSortOrder, string CurrentSort, SortDirection? sortDirection, int? page)
        {
            if (newSortOrder.Equals(CurrentSort))
            {
                if (page.HasValue) { return; }
                if (sortDirection == SortDirection.Ascending)
                {
                    ViewBag.SortDirection = SortDirection.Descending;
                }
                else
                {
                    ViewBag.SortDirection = SortDirection.Ascending;
                }
            }
            else
            {
                ViewBag.SortDirection = SortDirection.Ascending;
            }
        }

        private string GetPropertyValue(Listing m, string sortOrder)
        {
            var special = new List<string>() {"CurrentPrice","SqFtHeated"};
            object val=null;
            if (special.Contains(sortOrder))
            {
                sortOrder += "Value";
                var mth = m.GetType().GetMethod(sortOrder, BindingFlags.Instance | BindingFlags.Public);
                val = mth.Invoke(m,null);
            }
            else
            {
                var prop = m.GetType().GetProperty(sortOrder, BindingFlags.Instance | BindingFlags.Public);
                val = prop.GetValue(m);
            }
            
            string valStr = null;
            if(val!=null)
            {
                valStr = val.ToString();
            }
            return valStr;
        }


        //public object Index(int? page)
        //{
        //    var products = db.Listings; //returns IQueryable<Product> representing an unknown number of products. a thousand maybe?

        //    var pageNumber = page ?? 1; // if no page was specified in the querystring, default to the first page (1)
        //    var onePageOfProducts = 
        //        products.OrderBy(x=>x.MLNumber).ToPagedList(pageNumber, 25); // will only contain 25 products max because of the pageSize

        //    ViewBag.OnePageOfProducts = onePageOfProducts;
        //    return View();
        //}


        // GET: Listings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Listing mlsRow = db.Listings.Find(id);
            if (mlsRow == null)
            {
                return HttpNotFound();
            }
            return View(mlsRow);
        }

        // GET: Listings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Listings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,MLNumber,Status,Address,City,PostalCode,LegalSubdivisionName,SqFtHeated,CurrentPrice,Beds,FullBaths,HalfBaths,YearBuilt,Pool,PropertyStyle,Taxes,CDOM,ADOM,DaystoContract,SoldTerms,CloseDate,LPSqFt,SPSqFt,SPLP,ListOfficeName,ListAgentFullName,ListAgentID,SellingAgentName,SellingOfficeID,SellingAgentID,LSCListSide,OfficePrimaryBoardID")] Listing mlsRow)
        {
            if (ModelState.IsValid)
            {
                db.Listings.Add(mlsRow);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mlsRow);
        }

        // GET: Listings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Listing mlsRow = db.Listings.Find(id);
            if (mlsRow == null)
            {
                return HttpNotFound();
            }
            return View(mlsRow);
        }

        // POST: Listings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,MLNumber,Status,Address,City,PostalCode,LegalSubdivisionName,SqFtHeated,CurrentPrice,Beds,FullBaths,HalfBaths,YearBuilt,Pool,PropertyStyle,Taxes,CDOM,ADOM,DaystoContract,SoldTerms,CloseDate,LPSqFt,SPSqFt,SPLP,ListOfficeName,ListAgentFullName,ListAgentID,SellingAgentName,SellingOfficeID,SellingAgentID,LSCListSide,OfficePrimaryBoardID")] Listing mlsRow)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mlsRow).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mlsRow);
        }

        // GET: Listings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Listing mlsRow = db.Listings.Find(id);
            if (mlsRow == null)
            {
                return HttpNotFound();
            }
            return View(mlsRow);
        }

        // POST: Listings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Listing mlsRow = db.Listings.Find(id);
            db.Listings.Remove(mlsRow);
            db.SaveChanges();
            return RedirectToAction("Index");
        }        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        internal List<Listing> Search(FlippedHouse flippedHouse)
        {
            var list =  
            (from el in db.Listings
            where flippedHouse.Address.ToLower().Trim() == el.Address.ToLower().Trim() &&
                  flippedHouse.City.ToLower().Trim() == el.City.ToLower().Trim() &&
                  flippedHouse.PostalCode.ToLower().Trim() == el.PostalCode.ToLower().Trim() &&
                  el.Status.ToLower() == "sld"
            select el)/*.OrderBy(x=>x.CloseDate)*/.ToList();
            return list;
        }        
    }
}
