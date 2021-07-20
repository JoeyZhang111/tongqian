import pytest,requests
from JFGetCode.jf_auto import testjf
from JFGetCode.Excel import Getexcel
class Testjf:
    @pytest.mark.parametrize('expData', Getexcel('getcode'))  # 数据驱动
    def test_gene(self, expData):
        res = requests.post('http://bfyx.kabapay.com/ExtractPointCards/getcodedata', data=expData[0])
        # res = test_gene('http://192.168.0.46:8097/Service/Generate') # 获取响应数据---字典格式
        r = res.json()
        print(res.json())
        # 3- 预期结果--excel里与实际结果对比
        # print(response_data)
        assert r['ResponseMessage'] == expData[1]['ResponseMessage']

if __name__ == '__main__':
    resp = Getexcel('getcode')
    #pytest.main(['test_case.py', '-s', '--html=reports/test_gene.html'])  # ,'--alluredir=reports/allure'