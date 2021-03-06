﻿using Core.Constants;
using Core.DomainModel;
using Core.Interface.Repository;
using Core.Interface.Service;
using Core.Interface.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Service.Service
{
    public class CoreIdentificationDetailService : ICoreIdentificationDetailService
    {
        private ICoreIdentificationDetailRepository _repository;
        private ICoreIdentificationDetailValidator _validator;
        public CoreIdentificationDetailService(ICoreIdentificationDetailRepository _coreIdentificationDetailRepository, ICoreIdentificationDetailValidator _coreIdentificationDetailValidator)
        {
            _repository = _coreIdentificationDetailRepository;
            _validator = _coreIdentificationDetailValidator;
        }

        public ICoreIdentificationDetailValidator GetValidator()
        {
            return _validator;
        }

        public ICoreIdentificationDetailRepository GetRepository()
        {
            return _repository;
        }

        public IList<CoreIdentificationDetail> GetAll()
        {
            return _repository.GetAll();
        }

        public IList<CoreIdentificationDetail> GetObjectsByCoreIdentificationId(int CoreIdentificationId)
        {
            return _repository.GetObjectsByCoreIdentificationId(CoreIdentificationId);
        }

        public IList<CoreIdentificationDetail> GetObjectsByCoreBuilderId(int CoreBuilderId)
        {
            return _repository.GetObjectsByCoreBuilderId(CoreBuilderId);
        }

        public IList<CoreIdentificationDetail> GetObjectsByRollerTypeId(int rollerTypeId)
        {
            return _repository.GetObjectsByRollerTypeId(rollerTypeId);
        }

        public IList<CoreIdentificationDetail> GetObjectsByMachineId(int machineId)
        {
            return _repository.GetObjectsByMachineId(machineId);
        }

        public CoreIdentificationDetail GetObjectById(int Id)
        {
            return _repository.GetObjectById(Id);
        }

        public CoreIdentificationDetail GetObjectByDetailId(int CoreIdentificationId, int DetailId)
        {
            return _repository.Find(x => x.CoreIdentificationId == CoreIdentificationId && x.DetailId == DetailId && !x.IsDeleted);
        }

        public Item GetCore(CoreIdentificationDetail coreIdentificationDetail, ICoreBuilderService _coreBuilderService)
        {
            Item core = (coreIdentificationDetail.MaterialCase == Core.Constants.Constant.MaterialCase.New) ?
                        _coreBuilderService.GetNewCore(coreIdentificationDetail.CoreBuilderId) :
                        _coreBuilderService.GetUsedCore(coreIdentificationDetail.CoreBuilderId);
            return core;
        }

        public CoreIdentificationDetail CreateObject(int CoreIdentificationId, int DetailId, int MaterialCase, int CoreBuilderId, int RollerTypeId,
                                                     int MachineId, decimal RD, decimal CD, decimal RL, decimal WL, decimal TL, ICoreIdentificationService _coreIdentificationService,
                                                     ICoreBuilderService _coreBuilderService, IRollerTypeService _rollerTypeService, IMachineService _machineService)
        {
            CoreIdentificationDetail coreIdentificationDetail = new CoreIdentificationDetail
            {
                CoreIdentificationId = CoreIdentificationId,
                DetailId = DetailId,
                MaterialCase = MaterialCase,
                CoreBuilderId = CoreBuilderId,
                RollerTypeId = RollerTypeId,
                MachineId = MachineId,
                RD = RD,
                CD = CD,
                RL = RL,
                WL = WL,
                TL = TL
            };
            return this.CreateObject(coreIdentificationDetail, _coreIdentificationService, _coreBuilderService, _rollerTypeService, _machineService);
        }

        public CoreIdentificationDetail CreateObject(CoreIdentificationDetail coreIdentificationDetail, ICoreIdentificationService _coreIdentificationService,
                                                     ICoreBuilderService _coreBuilderService, IRollerTypeService _rollerTypeService, IMachineService _machineService)
        {
            coreIdentificationDetail.Errors = new Dictionary<String, String>();
            return (_validator.ValidCreateObject(coreIdentificationDetail, _coreIdentificationService, this, _coreBuilderService, _rollerTypeService, _machineService) ?
                    _repository.CreateObject(coreIdentificationDetail) : coreIdentificationDetail);
        }

        public CoreIdentificationDetail UpdateObject(CoreIdentificationDetail coreIdentificationDetail, ICoreIdentificationService _coreIdentificationService,
                                                     ICoreBuilderService _coreBuilderService, IRollerTypeService _rollerTypeService, IMachineService _machineService)
        {
            return (coreIdentificationDetail = _validator.ValidUpdateObject(coreIdentificationDetail, _coreIdentificationService, this, _coreBuilderService, _rollerTypeService, _machineService) ?
                                               _repository.UpdateObject(coreIdentificationDetail) : coreIdentificationDetail);
        }

        public CoreIdentificationDetail SoftDeleteObject(CoreIdentificationDetail coreIdentificationDetail, ICoreIdentificationService _coreIdentificationService,
                                                         IRecoveryOrderDetailService _recoveryOrderDetailService, IRollerWarehouseMutationDetailService _rollerWarehouseMutationDetailService)
        {
            return (coreIdentificationDetail = _validator.ValidDeleteObject(coreIdentificationDetail, _coreIdentificationService, _recoveryOrderDetailService, _rollerWarehouseMutationDetailService) ?
                                               _repository.SoftDeleteObject(coreIdentificationDetail) : coreIdentificationDetail);
        }

        public CoreIdentificationDetail SetJobScheduled(CoreIdentificationDetail coreIdentificationDetail, IRecoveryOrderService _recoveryOrderService, IRecoveryOrderDetailService _recoveryOrderDetailService)
        {
            return (coreIdentificationDetail = _validator.ValidSetJobScheduled(coreIdentificationDetail, _recoveryOrderService, _recoveryOrderDetailService) ? _repository.SetJobScheduled(coreIdentificationDetail) : coreIdentificationDetail);
        }

        public CoreIdentificationDetail UnsetJobScheduled(CoreIdentificationDetail coreIdentificationDetail, IRecoveryOrderService _recoveryOrderService, IRecoveryOrderDetailService _recoveryOrderDetailService)
        {
            return (coreIdentificationDetail = _validator.ValidUnsetJobScheduled(coreIdentificationDetail, _recoveryOrderService, _recoveryOrderDetailService) ? _repository.UnsetJobScheduled(coreIdentificationDetail) : coreIdentificationDetail);
        }

        public CoreIdentificationDetail FinishObject(CoreIdentificationDetail coreIdentificationDetail, DateTime FinishedDate, ICoreIdentificationService _coreIdentificationService,
                                                     ICoreBuilderService _coreBuilderService, IStockMutationService _stockMutationService,
                                                     IItemService _itemService, IBarringService _barringService, IWarehouseItemService _warehouseItemService)
        {
            coreIdentificationDetail.FinishedDate = FinishedDate;
            if( _validator.ValidFinishObject(coreIdentificationDetail, _coreIdentificationService, this, _coreBuilderService, _warehouseItemService))
            {
                CoreIdentification coreIdentification = _coreIdentificationService.GetObjectById(coreIdentificationDetail.CoreIdentificationId);
                if (coreIdentification.ContactId != null)
                {
                    // add contact core
                    int MaterialCase = coreIdentificationDetail.MaterialCase;
                    Item item = (MaterialCase == Core.Constants.Constant.MaterialCase.New ?
                                    _coreBuilderService.GetNewCore(coreIdentificationDetail.CoreBuilderId) :
                                    _coreBuilderService.GetUsedCore(coreIdentificationDetail.CoreBuilderId));
                    WarehouseItem warehouseItem = _warehouseItemService.FindOrCreateObject(coreIdentification.WarehouseId, item.Id);
                    StockMutation stockMutation = _stockMutationService.CreateStockMutationForCoreIdentification(coreIdentificationDetail, warehouseItem);
                    _stockMutationService.StockMutateObject(stockMutation, _itemService, _barringService, _warehouseItemService);
                }
                _repository.FinishObject(coreIdentificationDetail);
            }
            return coreIdentificationDetail;
        }

        public CoreIdentificationDetail UnfinishObject(CoreIdentificationDetail coreIdentificationDetail, ICoreIdentificationService _coreIdentificationService,
                                                     ICoreBuilderService _coreBuilderService, IStockMutationService _stockMutationService,
                                                     IItemService _itemService, IBarringService _barringService, IWarehouseItemService _warehouseItemService)
        {
            if (_validator.ValidUnfinishObject(coreIdentificationDetail, _coreIdentificationService, this, _coreBuilderService, _warehouseItemService))
            {
                CoreIdentification coreIdentification = _coreIdentificationService.GetObjectById(coreIdentificationDetail.CoreIdentificationId);
                if (coreIdentification.ContactId != null)
                {
                    // reduce contact core
                    int MaterialCase = coreIdentificationDetail.MaterialCase;
                    Item item = (MaterialCase == Core.Constants.Constant.MaterialCase.New ?
                                    _coreBuilderService.GetNewCore(coreIdentificationDetail.CoreBuilderId) :
                                    _coreBuilderService.GetUsedCore(coreIdentificationDetail.CoreBuilderId));
                    WarehouseItem warehouseItem = _warehouseItemService.FindOrCreateObject(coreIdentification.WarehouseId, item.Id);
                    IList<StockMutation> stockMutations = _stockMutationService.SoftDeleteStockMutationForCoreIdentification(coreIdentificationDetail, warehouseItem);
                    foreach (var stockMutation in stockMutations)
                    {
                        _stockMutationService.ReverseStockMutateObject(stockMutation, _itemService, _barringService, _warehouseItemService);
                    }
                }
                _repository.UnfinishObject(coreIdentificationDetail);
            }
            return coreIdentificationDetail;
        }

        public CoreIdentificationDetail BuildRoller(CoreIdentificationDetail coreIdentificationDetail)
        {
            return _repository.BuildRoller(coreIdentificationDetail);
        }

        public CoreIdentificationDetail DeliverObject(CoreIdentificationDetail coreIdentificationDetail, ICoreIdentificationService _coreIdentificationService, IRollerWarehouseMutationDetailService _rollerWarehouseMutationDetailService)
        {
            if (_validator.ValidDeliverObject(coreIdentificationDetail, _rollerWarehouseMutationDetailService))
            {
                _repository.DeliverObject(coreIdentificationDetail);

                CoreIdentification coreIdentification = _coreIdentificationService.GetObjectById(coreIdentificationDetail.CoreIdentificationId);
                if (_coreIdentificationService.GetValidator().ValidCompleteObject(coreIdentification, this))
                {
                    _coreIdentificationService.CompleteObject(coreIdentification, this);
                }
            }
            return coreIdentificationDetail;
        }

        public CoreIdentificationDetail UndoDeliverObject(CoreIdentificationDetail coreIdentificationDetail, ICoreIdentificationService _coreIdentificationService, IRollerWarehouseMutationDetailService _rollerWarehouseMutationDetailService)
        {
            if (_validator.ValidUndoDeliverObject(coreIdentificationDetail, _coreIdentificationService, _rollerWarehouseMutationDetailService))
            {
                _repository.UndoDeliverObject(coreIdentificationDetail);
            }
            return coreIdentificationDetail;
        }

        public bool DeleteObject(int Id)
        {
            return _repository.DeleteObject(Id);
        }
    }
}