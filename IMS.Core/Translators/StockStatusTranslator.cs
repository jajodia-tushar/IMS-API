using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public class StockStatusTranslator
    {
        public static Contracts.StockStatusResponse ToDataContractsObject(Entities.StockStatusResponse entityStockStatusResponse)
        {
            Contracts.StockStatusResponse contractStockStatusResponse = new Contracts.StockStatusResponse();
            if(entityStockStatusResponse.Status==Entities.Status.Success)
            {
                contractStockStatusResponse.Status = Contracts.Status.Success;
                contractStockStatusResponse.StockStatus = ToDataContractsObject(entityStockStatusResponse.StockStatus);
            }
            else
            {
                contractStockStatusResponse.Status = Contracts.Status.Failure;
                contractStockStatusResponse.Error = Translator.ToDataContractsObject(entityStockStatusResponse.Error);
            }
            return contractStockStatusResponse;
        }

        public static Dictionary<Contracts.Item, List<Contracts.StoreColourQuantity>> ToDataContractsObject(Dictionary<Entities.Item, List<Entities.StoreColourQuantity>> stockStatus)
        {
            Dictionary<Contracts.Item, List<Contracts.StoreColourQuantity>> contractStockStatus = new Dictionary<Contracts.Item, List<Contracts.StoreColourQuantity>>();
            if(contractStockStatus!=null && contractStockStatus.Count>0 )
            {
                foreach(Entities.Item item in stockStatus.Keys )
                {
                    if(item!=null && stockStatus[item]!=null && stockStatus[item].Count>0)
                    {
                        contractStockStatus.Add(Translator.ToDataContractsObject(item), ToDataContractsObject(stockStatus[item]));
                    }
                }
            }
            return contractStockStatus;
        }

        public static List<Contracts.StoreColourQuantity> ToDataContractsObject(List<Entities.StoreColourQuantity> list)
        {
            List<Contracts.StoreColourQuantity> contractList = new List<Contracts.StoreColourQuantity>();
            foreach(Entities.StoreColourQuantity storeColourQuantity in list)
            {
                if(storeColourQuantity!=null)
                {
                    contractList.Add(ToDataContractsObject(storeColourQuantity));
                }
            }
            return contractList;
        }

        public static Contracts.StoreColourQuantity ToDataContractsObject(Entities.StoreColourQuantity storeColourQuantity)
        {
            return new Contracts.StoreColourQuantity()
            {
                Colour=ToDataContractsObject(storeColourQuantity.Colour),
                StoreName=storeColourQuantity.StoreName,
                Quantity=storeColourQuantity.Quantity
            };
        }

        public static Contracts.Colour ToDataContractsObject(Entities.Colour colour)
        {
            switch (colour)
            {
                case Entities.Colour.Red: return Contracts.Colour.Red;
                case Entities.Colour.Amber: return Contracts.Colour.Amber;
            }
            return Contracts.Colour.Green;
        }
    }
}
