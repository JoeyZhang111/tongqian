import pytest,requests
from Generate.gene import test_gene
from Generate.getexcel import exceldata
#resList=exceldata('api','Generate')

#resp = exceldata('api', 'Generate')
#for i in resp:
   # testData = i[0]
    #print(testData)
class TestGenerate:
    @pytest.mark.parametrize('expData',exceldata('api', 'Generate'))#数据驱动
    def test_gene(self,expData):
        res=requests.post('http://192.168.0.46:8097/Service/Generate',data=expData[0])
        #res = test_gene('http://192.168.0.46:8097/Service/Generate') # 获取响应数据---字典格式
        r=res.json()
        #print(res.json())
        # 3- 预期结果--excel里与实际结果对比
        #print(response_data)
        assert r['ResponseMessage'] == expData[1]['ResponseMessage']
if __name__ == '__main__':
    pytest.main(['test_case.py','-s','--html=reports/test_gene.html'])#,'--alluredir=reports/allure'
