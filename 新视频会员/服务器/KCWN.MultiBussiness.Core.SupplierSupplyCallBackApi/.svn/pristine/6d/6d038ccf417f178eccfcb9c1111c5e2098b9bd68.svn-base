using KCWN.MultiBussiness.Core.Bussiness;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KCWN.MultiBussiness.Core.OModel.DBModel;
using KCWN.MultiBussiness.Core.OModel.V_Model;
using KCWN.MultiBussiness.Core.OModel.QueueModel;
using KCWN.MultiBussiness.Core.OModel;
using KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi.Common;
using MultiBus.ILog.Model;

namespace KCWN.MultiBussiness.Core.SupplierSupplyCallBackApi
{
    public abstract class ICallBack : Base
    {
        public abstract ResultObj Handle<T>(T entity);
        public T_OrdersChargeRecord QueryOrdersChargeRecordByID(string ChargeID)
        {
            OB_OrdersChargeRecord b_OrdersChargeRecord = new OB_OrdersChargeRecord();
            var item = b_OrdersChargeRecord.QueryOrderChargeRecordByID(ChargeID);
            return item;
        }

        public T_OrdersChargeRecord QueryOrderChargeRecordByChannelSerialNumber(Guid SupplierSupplyID, string ChannelSerialNumber)
        {
            OB_OrdersChargeRecord b_OrdersChargeRecord = new OB_OrdersChargeRecord();
            var item = b_OrdersChargeRecord.QueryOrderChargeRecordByChannelSerialNumber(SupplierSupplyID, ChannelSerialNumber);
            return item;
        }

        public SupplierSupplyInfo QuerySupplyInfo(Guid SupplierSupplyID)
        {
            try
            {
                SupplierSupplyInfo result = new SupplierSupplyInfo();
                OB_SupplierAttr b_SupplierAttr = new OB_SupplierAttr();
                OB_Supplier b_Supplier = new OB_Supplier();
                result.SupplierSupply = b_Supplier.GetSupplierSupplyInfoById(SupplierSupplyID);
                result.SupplierSupplyAttr = b_SupplierAttr.GetSuppliersSupplyAttr(SupplierSupplyID);
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public T_Products QueryProduct(long OrderID)
        {
            try
            {
                OB_Orders b_Order = new OB_Orders();
                T_Orders orders = b_Order.QueryOrderById(OrderID);
                if (orders != null)
                {
                    OB_Products b_Products = new OB_Products();
                    return b_Products.GetProductById(orders.ProductID);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool SaveChargeResult(T_OrdersChargeRecord entity, OrderChargeResult chargeResult)
        {
            try
            {
                if (entity == null || chargeResult == null || (chargeResult.ChargeState != OModel.EnumInfo.ChargeState.Success && chargeResult.ChargeState != OModel.EnumInfo.ChargeState.Fail))
                {
                    return false;
                }
                chargeResult.ChargeMeessage = chargeResult.ChargeMeessage == null ? "【回调】" : $"【回调】{chargeResult.ChargeMeessage}";

                OB_Orders b_Orders = new OB_Orders();
                T_Orders order = b_Orders.QueryOrderById(entity.OrderId);
                if (order == null)
                {
                    return false;
                }
                OrderQueryAndCallbackProcessHashModel item = CommQueueApi.GetOrderQueryAndCallbackProcessHash(entity.OrderId.ToString());
                List<Guid> SupplierSupplyIds = null;
                int SupplierSupplyIndex = 0;
                if (item != null)
                {
                    SupplierSupplyIds = item.SupplierSupplyIds;
                    SupplierSupplyIndex = item.SupplierSupplyIndex;
                }
                else
                {
                    //为空代表这个订单已经被处理或已经在处理中
                    SupplierSupplyIds = new List<Guid>() { entity.SupplierSupplyID };
                    SupplierSupplyIndex = 0;
                }
                if (SupplierSupplyIds[SupplierSupplyIndex] != entity.SupplierSupplyID)
                {//哈希值对应当前通知订单的供货商id不一致时候，只处理当前供货商
                    return false;//不处理（可能存在第一个供货商回调结果，但是订单已经处理到第个二供货渠道）
                }
                CommQueueApi.PushSupplierChargeResultDispacherQueue(new SupplierChargeResultDispacherQueueModel()
                {
                    OrderChargeResult = chargeResult,
                    NowSupplierSupplyId = entity.SupplierSupplyID,//供货商货源Id
                    SupplierSupplyIds = SupplierSupplyIds,
                    SupplierSupplyIndex = SupplierSupplyIndex,
                    ChargeId = entity.ChargeRecordId,
                    OrderInfo = order
                });
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }



    }


    public class Base
    {

    }

    public class ResultObj
    {
        public Object ResponseData { get; set; }
        public bool? IsSuccess { get; set; }
        public string Code { get; set; }
        public string Msg { get; set; }
    }
    public class SupplierSupplyInfo
    {
        public T_SupplierSupply SupplierSupply { get; set; }
        public List<T_SupplierSupplyAttr> SupplierSupplyAttr { get; set; }
    }
}
