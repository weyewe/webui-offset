﻿using Core.DomainModel;
using Core.Interface.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Interface.Service
{
    public interface ICoreBuilderService
    {
        ICoreBuilderValidator GetValidator();
        IList<CoreBuilder> GetAll();
        IList<CoreBuilder> GetObjectsByItemId(int ItemId);
        CoreBuilder GetObjectById(int Id);
        Item GetUsedCore(int id);
        Item GetNewCore(int id);
        CoreBuilder CreateObject(CoreBuilder coreBuilder, IUoMService _uomService, IItemService _itemService, IItemTypeService _itemTypeService,
                                 IWarehouseItemService _warehouseItemService, IWarehouseService _warehouseService);
        CoreBuilder CreateObject(string BaseSku, string SkuNewCore, string SkuUsedCore, string Name, string Category, int UoMId,
                                 IUoMService _uomService, IItemService _itemService, IItemTypeService _itemTypeService,
                                 IWarehouseItemService _warehouseItemService, IWarehouseService _warehouseService);
        CoreBuilder UpdateObject(CoreBuilder coreBuilder, IUoMService _uomService, IItemService _itemService, IItemTypeService _itemTypeService);
        CoreBuilder SoftDeleteObject(CoreBuilder coreBuilder, IItemService _itemService, IRollerBuilderService _rollerBuilderService,
                                     ICoreIdentificationDetailService _coreIdentificationDetailService, IRecoveryOrderDetailService _recoveryOrderDetailService,
                                     IRecoveryAccessoryDetailService _recoveryAccessoryDetailService, IWarehouseItemService _warehouseItemService,
                                     IStockMutationService _stockMutationService, IItemTypeService _itemTypeService);
        bool DeleteObject(int Id);
        bool IsBaseSkuDuplicated(CoreBuilder coreBuilder);
    }
}