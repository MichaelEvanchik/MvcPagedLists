
using mediaradar.AdDataService;
using mediaradar.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList.Mvc;
using PagedList;

namespace mediaradar.Controllers
{
    public class HomeController : Controller
    {
        //this doenst change so it can go here
        DateTime dtStart = Convert.ToDateTime("1/1/2011");
        DateTime dtEnd = Convert.ToDateTime("4/1/2011");
        DefaultCacheProvider Cache = new DefaultCacheProvider();

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult All(string sortOrder, int? page)
        {
            //fyi stuff
            //because this is a webservice the class is in server references, to conform to MVC we should use a model in the Model folder, but it also would add extra overhead
            //but there is no rule the class has to be in models so i left it alone and saved the memory copy
            //mediaradar.Models.AdModel[] model;
            //model = (mediaradar.Models.AdModel[])modelAds;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.AddIDSortParm = sortOrder == "ad_id_asc" ? "ad_id_desc" : "ad_id_asc";
            ViewBag.BrandIDSortParm = sortOrder == "brand_id_asc" ? "brand_id_desc" : "brand_id_asc";
            ViewBag.BrandNameSortParm = sortOrder == "brand_name_asc" ? "brand_name_desc" : "brand_name_asc";
            ViewBag.NumOfPagesSortParm = sortOrder == "number_of_pages_asc" ? "number_of_pages_desc" : "number_of_pages_asc";
            ViewBag.PositionSortParm = sortOrder == "position_asc" ? "position_desc" : "position_asc";

            mediaradar.AdDataService.Ad[] modelAds = new mediaradar.AdDataService.Ad[1];

            try
            {
                //i used a cache globally for all services, it would probably be faster if these where stored by action name for better perm but more memory, depends on real usage
                modelAds = (mediaradar.AdDataService.Ad[])Cache.Get("ads");
            }
            catch { }

            if (modelAds != null)
            {
                //default is by brand
                //even though i used the cache, its global so have to make sure its sorted correctly by default
                //and even more so below
                var sortedAds = from s in modelAds
                                orderby s.Brand.BrandName
                                select s;

                switch (sortOrder)
                {
                    case "ad_id_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.AdId);
                        break;
                    case "ad_id_asc":
                        sortedAds = sortedAds.ToList().OrderBy(x => x.AdId);
                        break;
                    case "brand_id_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.Brand.BrandId);
                        break;
                    case "brand_id_asc":
                        sortedAds = sortedAds.ToList().OrderBy(x => x.Brand.BrandId);
                        break;
                    case "brand_name_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.Brand.BrandName);
                        break;
                    case "brand_name_asc":
                        //default above just break
                        break;
                    case "number_of_pages_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.NumPages);
                        break;
                    case "number_of_pages_asc":
                        sortedAds = sortedAds.ToList().OrderBy(x => x.NumPages);
                        break;
                    case "position_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.Position);
                        break;
                    case "position_asc":
                        sortedAds = sortedAds.ToList().OrderBy(x => x.Position);
                        break;
                }
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(sortedAds.ToPagedList(pageNumber, pageSize));
            }
            else
            {//cache timeout read again from service, or this is the first time
                var wcf = new AdDataServiceClient();

                var modelAdsFirstTime = wcf.GetAdDataByDateRange(dtStart, dtEnd);

                var sortedAds = from s in modelAdsFirstTime
                                orderby s.Brand.BrandName
                                select s;

                switch (sortOrder)
                {
                    case "ad_id_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.AdId);
                        break;
                    case "ad_id_asc":
                        sortedAds = sortedAds.ToList().OrderBy(x => x.AdId);
                        break;
                    case "brand_id_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.Brand.BrandId);
                        break;
                    case "brand_id_asc":
                        sortedAds = modelAds.ToList().OrderBy(x => x.Brand.BrandId);
                        break;
                    case "brand_name_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.Brand.BrandName);
                        break;
                    case "brand_name_asc":
                        //default above just break
                        break;
                    case "number_of_pages_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.NumPages);
                        break;
                    case "number_of_pages_asc":
                        sortedAds = sortedAds.ToList().OrderBy(x => x.NumPages);
                        break;
                    case "position_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.Position);
                        break;
                    case "position_asc":
                        sortedAds = sortedAds.ToList().OrderBy(x => x.Position);
                        break;
                }

                if (modelAdsFirstTime.Any())
                {
                    // Put this data into the cache for 10 minutes wasnt in specs, but makes more sense then completely disconnected data
                    //could have saved this by action in the key for better perm but more memory, depends on real usage
                    Cache.Set("ads", modelAdsFirstTime, 10);
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(sortedAds.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult TopCover(string sortOrder, int? page)
        {

            ViewBag.AddIDSortParm = sortOrder == "ad_id_asc" ? "ad_id_desc" : "ad_id_asc";
            ViewBag.BrandIDSortParm = sortOrder == "brand_id_asc" ? "brand_id_desc" : "brand_id_asc";
            ViewBag.BrandNameSortParm = sortOrder == "brand_name_asc" ? "brand_name_desc" : "brand_name_asc";
            ViewBag.NumOfPagesSortParm = sortOrder == "number_of_pages_asc" ? "number_of_pages_desc" : "number_of_pages_asc";
            ViewBag.PositionSortParm = sortOrder == "position_asc" ? "position_desc" : "position_asc";

            mediaradar.AdDataService.Ad[] modelAds = new mediaradar.AdDataService.Ad[1];

            try
            {
                //i used a cache globally for all services, it would probably be faster if these where stored by action name for better perm but more memory, depends on real usage
                modelAds = (mediaradar.AdDataService.Ad[])Cache.Get("ads");
            }
            catch { }

            if (modelAds != null)
            {

                //default is by brand
                //even though i used the cache, its global so have to make sure its sorted correctly by default
                //and even more so below
                //var sortedAds = modelAds.ToList().Where(ab => ab.Position == "Cover" && ab.NumPages > (decimal).5).OrderBy(x => x.Brand.BrandName);
                var sortedAds = from s in modelAds
                                where s.Position == "Cover" && s.NumPages >= (decimal).5
                                orderby s.Brand.BrandName
                                select s;

                switch (sortOrder)
                {
                    case "ad_id_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.AdId);
                        break;
                    case "ad_id_asc":
                        sortedAds = sortedAds.ToList().OrderBy(x => x.AdId);
                        break;
                    case "brand_id_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.Brand.BrandId);
                        break;
                    case "brand_id_asc":
                        sortedAds = sortedAds.ToList().OrderBy(x => x.Brand.BrandId);
                        break;
                    case "brand_name_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.Brand.BrandName);
                        break;
                    case "brand_name_asc":
                        //default above just break
                        break;
                    case "number_of_pages_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.NumPages);
                        break;
                    case "number_of_pages_asc":
                        sortedAds = sortedAds.ToList().OrderBy(x => x.NumPages);
                        break;
                    case "position_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.Position);
                        break;
                    case "position_asc":
                        sortedAds = sortedAds.ToList().OrderBy(x => x.Position);
                        break;
                }
                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(sortedAds.ToPagedList(pageNumber, pageSize));
            }
            else
            {//cache timeout read again from service, or this is the first time
                var wcf = new AdDataServiceClient();

                var modelAdsFirstTime = wcf.GetAdDataByDateRange(dtStart, dtEnd);

                //default is by brand
                var sortedAds = from s in modelAdsFirstTime
                                where s.Position == "Cover" && s.NumPages >= (decimal).5
                                orderby s.Brand.BrandName
                                select s;

                switch (sortOrder)
                {
                    case "ad_id_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.AdId);
                        break;
                    case "ad_id_asc":
                        sortedAds = sortedAds.ToList().OrderBy(x => x.AdId);
                        break;
                    case "brand_id_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.Brand.BrandId);
                        break;
                    case "brand_id_asc":
                        sortedAds = sortedAds.ToList().OrderBy(x => x.Brand.BrandId);
                        break;
                    case "brand_name_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.Brand.BrandName);
                        break;
                    case "brand_name_asc":
                        //default above just break
                        break;
                    case "number_of_pages_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.NumPages);
                        break;
                    case "number_of_pages_asc":
                        sortedAds = sortedAds.ToList().OrderBy(x => x.NumPages);
                        break;
                    case "position_desc":
                        sortedAds = sortedAds.ToList().OrderByDescending(x => x.Position);
                        break;
                    case "position_asc":
                        sortedAds = sortedAds.ToList().OrderBy(x => x.Position);
                        break;
                }

                if (modelAdsFirstTime.Any())
                {
                    // Put this data into the cache for 10 minutes wasnt in specs, but makes more sense then completely disconnected data
                    //could have saved this by action in the key for better perm but more memory, depends on real usage
                    Cache.Set("ads", modelAdsFirstTime, 10);
                }

                int pageSize = 10;
                int pageNumber = (page ?? 1);

                return View(sortedAds.ToPagedList(pageNumber, pageSize));
            }
        }

        public ActionResult TopAds()
        {
            mediaradar.AdDataService.Ad[] modelAds = new mediaradar.AdDataService.Ad[1];

            try
            {
                //i used a cache globally for all services, it would probably be faster if these where stored by action name for better perm but more memory, depends on real usage
                modelAds = (mediaradar.AdDataService.Ad[])Cache.Get("ads");
            }
            catch { }

            if (modelAds != null)
            {
                //default is by brand
                //even though i used the cache, its global so have to make sure its sorted correctly by default
                //and even more so below

                //lets use an inherited model this time to be more PC
                var sortedAds = modelAds.Select(group =>
                     new mediaradar.Models.TopAdsModel
                     {
                         NumPages = group.NumPages,
                         Brand = group.Brand,
                         AdId = group.AdId,
                         Position = group.Position
                     })
                    .OrderBy(group => group.Brand.BrandName).OrderByDescending(group => group.NumPages).Take(5);


                return View(sortedAds);
            }
            else
            {//cache timeout read again from service, or this is the first time
                var wcf = new AdDataServiceClient();

                var modelAdsFirstTime = wcf.GetAdDataByDateRange(dtStart, dtEnd);

                //lets use an inherited model this time to be more PC
                var sortedAds = modelAdsFirstTime.Select(group =>
                        new mediaradar.Models.TopAdsModel
                        {
                            NumPages = group.NumPages,
                            Brand = group.Brand,
                            AdId = group.AdId,
                            Position = group.Position
                        })
                  .OrderBy(group => group.Brand.BrandName).OrderByDescending(group => group.NumPages).Take(5);

                if (modelAdsFirstTime.Any())
                {
                    // Put this data into the cache for 10 minutes wasnt in specs, but makes more sense then completely disconnected data
                    //could have saved this by action in the key for better perm but more memory, depends on real usage
                    Cache.Set("ads", modelAdsFirstTime, 10);
                }


                return View(sortedAds);
            }
        }

        public ActionResult TopBrands()
        {
            mediaradar.AdDataService.Ad[] modelAds = new mediaradar.AdDataService.Ad[1];

            try
            {
                //i used a cache globally for all services, it would probably be faster if these where stored by action name for better perm but more memory, depends on real usage
                modelAds = (mediaradar.AdDataService.Ad[])Cache.Get("ads");
            }
            catch { }

            if (modelAds != null)
            {
                //even though i used the cache, its global so have to make sure its sorted correctly by default
                //lets use an inherited model this time to be more PC
                var sortedAds = (from bs in (modelAds)
                                 group bs by bs.Brand.BrandId into g
                                 orderby g.Sum(x => x.NumPages) descending
                                 select new mediaradar.Models.TopAdsModel
                                 {
                                     AdId = g.First().AdId,
                                     Brand = g.First().Brand,
                                     NumPages = g.First().NumPages,
                                     Position = g.First().Position,
                                     SumOfPages = g.Sum(x => x.NumPages)
                                 }
                              ).Take(5);

                return View(sortedAds);
            }
            else
            {//cache timeout read again from service, or this is the first time
                var wcf = new AdDataServiceClient();

                var modelAdsFirstTime = wcf.GetAdDataByDateRange(dtStart, dtEnd);

                //lets use an inherited model this time to be more PC
                var sortedAds = (from bs in (modelAdsFirstTime)
                              group bs by bs.Brand.BrandId into g
                              orderby g.Sum(x => x.NumPages) descending
                              select new mediaradar.Models.TopAdsModel
                              {
                                  AdId = g.First().AdId,
                                  Brand = g.First().Brand,
                                  NumPages = g.First().NumPages,
                                  Position = g.First().Position,
                                  SumOfPages = g.Sum(x => x.NumPages)
                              }
                              ).Take(5);

                if (modelAdsFirstTime.Any())
                {
                    // Put this data into the cache for 10 minutes wasnt in specs, but makes more sense then completely disconnected data
                    //could have saved this by action in the key for better perm but more memory, depends on real usage
                    Cache.Set("ads", modelAdsFirstTime, 10);
                }

                return View(sortedAds);
            }
        }
    }
}