﻿using Core.DomainModel;
using Core.Interface.Repository;
using Core.Interface.Service;
using Core.Interface.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Service
{
    public class BarringService : IBarringService
    {
        private IBarringRepository _repository;
        private IBarringValidator _validator;
        public BarringService(IBarringRepository _barringRepository, IBarringValidator _barringValidator)
        {
            _repository = _barringRepository;
            _validator = _barringValidator;
        }

        public IBarringValidator GetValidator()
        {
            return _validator;
        }

        public IBarringRepository GetRepository()
        {
            return _repository;
        }

        public IList<Barring> GetAll()
        {
            return _repository.GetAll();
        }

        public IList<Barring> GetObjectsByItemTypeId(int ItemTypeId)
        {
            return _repository.GetObjectsByItemTypeId(ItemTypeId);
        }

        public IList<Barring> GetObjectsByUoMId(int UoMId)
        {
            return _repository.GetObjectsByUoMId(UoMId);
        }
        
        public IList<Barring> GetObjectsByMachineId(int MachineId)
        {
            return _repository.GetObjectsByMachineId(MachineId);
        }

        public IList<Barring> GetObjectsByContactId(int ContactId)
        {
            return _repository.GetObjectsByContactId(ContactId);
        }

        public Barring GetObjectById(int Id)
        {
            return _repository.GetObjectById(Id);
        }

        public Barring GetObjectBySku(string Sku)
        {
            return _repository.GetObjectBySku(Sku);
        }

        public Barring CreateObject(Barring barring, IBarringService _barringService, IUoMService _uomService, IItemService _itemService, IItemTypeService _itemTypeService,
                                    IContactService _contactService, IMachineService _machineService,
                                    IWarehouseItemService _warehouseItemService, IWarehouseService _warehouseService)
        {
            barring.Errors = new Dictionary<String, String>();
            if (_validator.ValidCreateObject(barring, _barringService, _uomService, _itemService, _itemTypeService, _contactService, _machineService))
            {
                barring = _repository.CreateObject(barring);
            }
            return barring;
        }

        public Barring UpdateObject(Barring barring, IBarringService _barringService, IUoMService _uomService, IItemService _itemService, IItemTypeService _itemTypeService,
                                    IContactService _contactService, IMachineService _machineService,
                                    IWarehouseItemService _warehouseItemService, IWarehouseService _warehouseService)
        {        
            return (barring = _validator.ValidUpdateObject(barring, _barringService, _uomService, _itemService, _itemTypeService, _contactService, _machineService) ?
                              _repository.UpdateObject(barring) : barring);
        }

        public Barring SoftDeleteObject(Barring barring, IItemTypeService _itemTypeService, IWarehouseItemService _warehouseItemService)
        {
            if (_validator.ValidDeleteObject(barring, _itemTypeService, _warehouseItemService))
            {
                IList<WarehouseItem> allwarehouseitems = _warehouseItemService.GetObjectsByItemId(barring.Id);
                foreach (var warehouseitem in allwarehouseitems)
                {
                    IWarehouseItemValidator warehouseItemValidator = _warehouseItemService.GetValidator();
                    if (!warehouseItemValidator.ValidDeleteObject(warehouseitem))
                    {
                        barring.Errors.Add("Generic", "Tidak bisa menghapus item yang berhubungan dengan warehouse");
                        return barring;
                    }
                }
                foreach (var warehouseitem in allwarehouseitems)
                {
                    _warehouseItemService.SoftDeleteObject(warehouseitem);
                }
                _repository.SoftDeleteObject(barring);
            }
            return barring;
        }

        public Barring AdjustQuantity(Barring barring, int quantity)
        {
            barring.Quantity += quantity;
            return (barring = _validator.ValidAdjustQuantity(barring) ? _repository.UpdateObject(barring) : barring);
        }

        public Barring AdjustPendingReceival(Barring barring, int quantity)
        {
            barring.PendingReceival += quantity;
            return (barring = _validator.ValidAdjustPendingReceival(barring) ? _repository.UpdateObject(barring) : barring);
        }

        public Barring AdjustPendingDelivery(Barring barring, int quantity)
        {
            barring.PendingDelivery += quantity;
            return (barring = _validator.ValidAdjustPendingDelivery(barring) ? _repository.UpdateObject(barring) : barring);
        }

        public bool DeleteObject(int Id)
        {
            return _repository.DeleteObject(Id);
        }

        public bool IsSkuDuplicated(Barring barring)
        {
            return _repository.IsSkuDuplicated(barring);
        }
    }
}