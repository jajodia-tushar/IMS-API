using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.Translators
{
    public static class ShelfTranslator
    {
        public static Contracts.ShelfResponse ToDataContractsObject(Entities.ShelfResponse entityShelfResponse)
        {
            Contracts.ShelfResponse shelfResponse = new Contracts.ShelfResponse();
            if (entityShelfResponse.Status == Entities.Status.Success)
            {
                shelfResponse.Status = Contracts.Status.Success;
                shelfResponse.Shelves = ToDataContractsObject(entityShelfResponse.Shelves);
            }
            else
            {
                shelfResponse.Status = Contracts.Status.Failure;
                shelfResponse.Error = Translator.ToDataContractsObject(entityShelfResponse.Error);
            }
            return shelfResponse;

        }

        public static Entities.Shelf ToEntitiesObject(Contracts.Shelf shelf)
        {
            return new Entities.Shelf()
            {
                Id = shelf.Id,
                Name = shelf.Name,
                IsActive = shelf.IsActive,
                Code = shelf.Code

            };
        }

        public static List<Contracts.Shelf> ToDataContractsObject(List<Entities.Shelf> shelfs)
        {
            var dtoShelves = new List<Contracts.Shelf>();
            foreach (var shelf in shelfs)
            {
                Contracts.Shelf dtoShelf = new Contracts.Shelf();
                dtoShelf.Id = shelf.Id;
                dtoShelf.Name = shelf.Name;
                dtoShelf.Code = shelf.Code;
                dtoShelf.IsActive = shelf.IsActive;
                dtoShelves.Add(dtoShelf);

            }
            return dtoShelves;

        }
        public static Contracts.Shelf ToDataContractsObject(Entities.Shelf doShelf)
        {
            var dtoShelf = new Contracts.Shelf();
            dtoShelf.Id = doShelf.Id;
            dtoShelf.Name = doShelf.Name;
            dtoShelf.IsActive = doShelf.IsActive;
            dtoShelf.Code = doShelf.Code;
            return dtoShelf;
        }
    }
}
