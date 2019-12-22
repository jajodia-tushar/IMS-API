using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public class RAGStatusTranslator
    {
        public static Contracts.RAGStatusResponse ToDataContractObject(Entities.RAGStatusResponse doRAGStatusResponse)
        {
            var dtoRAGStatusResponse = new Contracts.RAGStatusResponse();
            try
            {
                if (doRAGStatusResponse.Status.Equals(Entities.Status.Success))
                {
                    dtoRAGStatusResponse.RAGStatusList = ToDataContractObject(doRAGStatusResponse.RAGStatusList);
                    dtoRAGStatusResponse.Status = Contracts.Status.Success;
                }
                else
                {
                    dtoRAGStatusResponse.Status = Contracts.Status.Failure;
                    dtoRAGStatusResponse.Error = Translator.ToDataContractsObject(doRAGStatusResponse.Error);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return dtoRAGStatusResponse;
        }

        public static List<Contracts.RAGStatus> ToDataContractObject(List<Entities.RAGStatus> doRAGStatusList)
        {
            var dtoRAGStatusList = new List<Contracts.RAGStatus>();
            try
            {
                foreach (var doRAGStatus in doRAGStatusList)
                    dtoRAGStatusList.Add(ToDataContractObject(doRAGStatus));
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return dtoRAGStatusList;
        }

        public static Contracts.RAGStatus ToDataContractObject(Entities.RAGStatus doRAGStatus)
        {
            try
            {
                return new Contracts.RAGStatus()
                {
                    Name = doRAGStatus.Name,
                    Code = doRAGStatus.Code,
                    ColourCountMappings = ToDataContractObject(doRAGStatus.ColourCountMappings)
                };
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public static List<Contracts.ColourCountMapping> ToDataContractObject(List<Entities.ColourCountMapping> doColourCountMappings)
        {
            var dtoColourCountMappings = new List<Contracts.ColourCountMapping>();
            try
            {
                foreach (var doColourCountMapping in doColourCountMappings)
                    dtoColourCountMappings.Add(ToDataContractObject(doColourCountMapping));
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return dtoColourCountMappings;
        }

        public static Contracts.ColourCountMapping ToDataContractObject(Entities.ColourCountMapping doColourCountMapping)
        {
            try
            {
                return new Contracts.ColourCountMapping()
                {
                    Count = doColourCountMapping.Count,
                    Colour = ToDataContractObject(doColourCountMapping.Colour)
                };
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public static Contracts.Colour ToDataContractObject(Entities.Colour doColour)
        {
            try
            {
                switch (doColour)
                {
                    case Entities.Colour.Red:
                        return Contracts.Colour.Red;
                    case Entities.Colour.Amber:
                        return Contracts.Colour.Amber;
                    case Entities.Colour.Green:
                        return Contracts.Colour.Green;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return Contracts.Colour.Green;
        }
    }
}