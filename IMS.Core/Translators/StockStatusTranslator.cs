﻿using System;
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
                contractStockStatusResponse.NamesOfAllStores = entityStockStatusResponse.NamesOfAllStores;
                contractStockStatusResponse.StockStatusList = ToDataContractsObject(entityStockStatusResponse.StockStatusList);
            }
            else
            {
                contractStockStatusResponse.Status = Contracts.Status.Failure;
                contractStockStatusResponse.Error = Translator.ToDataContractsObject(entityStockStatusResponse.Error);
            }
            return contractStockStatusResponse;
        }

        public static List<Contracts.StockStatusList> ToDataContractsObject(List<Entities.StockStatusList> stockStatus)
        {
            List<Contracts.StockStatusList> contractStockStatusList = new List<Contracts.StockStatusList>();
            if(stockStatus!=null && stockStatus.Count>0 )
            {
                foreach(Entities.StockStatusList stockStatusList in stockStatus )
                {
                    Contracts.StockStatusList contractStockStatus = new Contracts.StockStatusList();

                    if (stockStatusList.Item!=null)
                    {
                        contractStockStatus.Item = Translator.ToDataContractsObject(stockStatusList.Item);
                    }
                    if(stockStatusList.StockStatus!=null && stockStatusList.StockStatus.Count>0)
                    {
                        contractStockStatus.StockStatus = ToDataContractsObject(stockStatusList.StockStatus);
                    }
                    contractStockStatusList.Add(contractStockStatus);
                }
            }
            return contractStockStatusList;
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
