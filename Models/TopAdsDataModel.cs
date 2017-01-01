using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mediaradar.Models
{
    public partial class TopAdsModel : mediaradar.AdDataService.Ad 
    {

        public decimal SumOfPages;

    }
    public partial class BrandModel :mediaradar.AdDataService.Brand,IComparable
    {
        public int CompareTo(object other)
        {
            if (other == null) return 1;
            else
                return 0;
        }
    }
}