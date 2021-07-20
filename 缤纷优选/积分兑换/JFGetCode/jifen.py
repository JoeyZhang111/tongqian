import datetime,time,requests
import hashlib
#随机生成时间戳
ct = time.time()                                        # 取得系统时间
local_time = time.localtime(ct)
date_head = time.strftime("%Y%m%d%H%M%S", local_time)   # 格式化时间
date_m_secs = str(datetime.datetime.now().timestamp()).split(".")[-1]    # 毫秒级时间戳
time_stamp = "%s%.3s" % (date_head, date_m_secs)
print(time_stamp)
class Order:
    def DirectOrder(url,indata):
      # 字典--修改值的操作
        #print(sign)
        res=requests.post(url,data=indata)
        return res.json()


if __name__ == '__main__':
    testdata={"merchant_id":"600000038253","mobile":"ab@_A","sub_order_id":time_stamp,"ext_goods_id": "17","buy_num":"1","expire_time":"2020-11-20 15:00:00","start_time":"2020-11-22 15:00:00","sign":"123"}
    print(testdata)
    r=Order.DirectOrder('http://bfyx.kabapay.com/ExtractPointCards/getcodedata',testdata)
    print(r)
