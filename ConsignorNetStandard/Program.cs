using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsignorNetStandard;
using QA = ConsignorNetStandard.no.consignorsupport.www;
using ConsignorNetStandard.no.consignorsupport.www;

namespace ConsignorNetStandard
{
    class Program
    {
        static void Main(string[] args)
        {

            var consignor = new Consignor();

            var dps = consignor.GetDropPoints("1523");

            foreach (var dp in dps)
            {
                Console.WriteLine(dp.Name1);
            }
            Console.Read();
        }
        /// <summary>
        /// Consignor web service wrapper
        /// </summary>
        public class Consignor
        {
            ShipAdvisorWS shipAdvisor;

            public Consignor()
            {
                shipAdvisor = new ShipAdvisorWS();
                shipAdvisor.ServiceAuthenticationHeaderValue = new QA.ServiceAuthenticationHeader();
                shipAdvisor.ServiceAuthenticationHeaderValue.Username = "SVEIS84380";
                shipAdvisor.ServiceAuthenticationHeaderValue.Password = "9j15l5kUR5a7SM";
            }

            /// <summary>
            /// Used to get the closest 5 droppointts for an arbitrary package going to @postCode
            /// </summary>
            public DropPointData[] GetDropPoints(string postCode)
            {
                return GetDropPoints(1, 5000, postCode, 5, DeliveryType.HomeDelivery);
            }
            
            /// <summary>
            /// Used to get the shipment options for a shipment of @nrOfPackages, weighing @weightInGrams headed for @postCode
            /// using @deliveryType
            /// </summary>
            public ProductsWrapper GetFreightOptions(int nrOfPackages, int weightInGrams, string postCode, DeliveryType deliveryType)
            {
                // Create the receiver
                Receiver Receiver = new Receiver
                {
                    PostCode = postCode,
                    CountryCode = "NO"
                };

                //Create WebshopLine( goods line)
                List<WebShopLine> lines = new List<WebShopLine>();
                WebShopLine ln = new WebShopLine
                {
                    NumberOfPackages = nrOfPackages,
                    PackageWeight = weightInGrams
                };
                lines.Add(ln);

                //Create shipment
                WebShopShipment shipment = new WebShopShipment
                {
                    WebShopId = 1209,
                    Shopper = Receiver,
                    CODAmount = 0,
                    Lines = lines.ToArray()
                };
                // Create an instance of ProductsWrapper class to hold the available products that is returned from ShipAdvisor
                ProductsWrapper FreightProducts = new ProductsWrapper();
                FreightProducts = shipAdvisor.GetFreightProductsForShipment(shipment, (int)deliveryType);

                return FreightProducts;
            }


            /// <summary>
            /// Used to get the @nrOfDropPoints droppoints for a shipment of @nrOfPackages, weighing @weightInGrams headed for @postCode
            /// using @deliveryType
            /// </summary>
            public DropPointData[] GetDropPoints(int nrOfPackages, int weightInGrams, string postCode, int nrOfDropPoints, DeliveryType deliveryType)
            {

                ProductsWrapper FreightProducts = GetFreightOptions(nrOfPackages, weightInGrams, postCode, deliveryType);

                var cID = FreightProducts.ProductInfoList[0].ConceptId;
                DropPointData[] dps = shipAdvisor.SearchForDropPoints((int)cID, "11685000025", "NO", "", postCode, "", nrOfDropPoints);
                

                return dps;
            }
            
        }
        public enum DeliveryType
        {
            HomeDelivery = 10,
            AtWork = 20,
            PickUp = 30
        }
    }
}
