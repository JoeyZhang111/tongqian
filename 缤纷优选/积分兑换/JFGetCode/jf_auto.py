from JFGetCode.Excel import Getexcel
import requests,json
def testjf():
    res=requests.post(url='http://bfyx.kabapay.com/ExtractPointCards/getcodedata',data=testData)
    return res.json()
if __name__ == '__main__':
    resp = Getexcel('getcode')
    for i in resp:
        testData = i[1]
        print(testData)
        res=testjf()
        #print(res)