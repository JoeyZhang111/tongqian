import xlrd,json,requests
#获取excel数据--请求体和预期
def exceldata(sheetName,caseName):
    '''sheetName表名
    caseName用例名
    return一个列表嵌套元组[(请求体),(期望)]'''
    reList=[]#存放结果
    excelDir='./短链接接口.xlsx'
    #打开excel对象
    workBook=xlrd.open_workbook(excelDir)
    #对某表操作
    workSheet=workBook.sheet_by_name(sheetName)
    #获取值第7列和第9列
    #print(workSheet.col_values(0))
    #获取数据
    id=0
    for i in workSheet.col_values(0):
        if caseName in i:
            #cell(行号，列号)
            body=workSheet.cell(id,6).value
            exp=workSheet.cell(id,8).value
            reList.append((json.loads(body),json.loads(exp)))#字符串--字典
        id += 1
    #return reList
    print(reList[0])
if __name__ == '__main__':
    res=exceldata('api','Generate')
  #  for i in res:
      #  a=i[0]
       # print(a)
       # r=requests.post('http://192.168.0.46:8097/Service/Generate',data=a)
       # print(r.json())

