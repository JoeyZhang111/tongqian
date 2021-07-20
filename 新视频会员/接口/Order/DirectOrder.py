import datetime,time,requests
import hashlib
#随机生成时间戳
t=time.time()
a=int(round(t * 1000))
print(a)
def md5value(key):
    md5 = hashlib.md5()  # 创建md5对象
    md5.update(key.encode('utf-8'))  # 加密方法
    sign=md5.hexdigest()  # 加密后的结果
    return sign.upper()
class Order:
    def DirectOrder(url,indata):
        payload=indata
        payload['sign'] = md5value(payload['sign'])  # 字典--修改值的操作
        #print(sign)
        res=requests.post(url,data=payload)
        return res.json()


if __name__ == '__main__':
    testdata={"AppKey":"211394680","TimesTamp":str(a),"ProductCode":"PLM100032","BuyCount": "1","MOrderID":str(a),"ChargeAccount":"185464543124","ChargeAccountType":"1","CustomerIP": "121.43.39.80","IsCallback":"1","CallBackUrl":"http://192.168.0.198:8033/CallBack/Test","Version":"1.0","AppSecret":"lOqVb8XGUkm1O/ppIxKhsg=="}
    sig = testdata["AppKey"]+testdata["BuyCount"]+testdata["CallBackUrl"]+testdata["ChargeAccount"]+testdata["CustomerIP"]+testdata["MOrderID"]+testdata["ProductCode"]+testdata["TimesTamp"]+testdata["Version"]+testdata["AppSecret"]
    testdata["sign"]=sig
    print(testdata)
    r=Order.DirectOrder("http://182.150.21.90:8050/Order/DirectOrder",testdata)
    print(r)
