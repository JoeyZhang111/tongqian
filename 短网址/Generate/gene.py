from Generate.getexcel import exceldata
import requests,json

def test_gene(url):
    res=requests.post(url,data=testData)
    return res.json()
if __name__ == '__main__':
    resp = exceldata('api', 'Generate')
    for i in resp:
        testData = i[0]
        #print(testData)
        res=test_gene('http://192.168.0.46:8097/Service/Generate')
        print(res)