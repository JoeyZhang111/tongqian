import xlrd,json
import requests
#获取excel数据--请求方式请求体和预期
def Getexcel(sheetName):
#打开表
    excel='./积分.xlsx'
#打开excel对象
    Book=xlrd.open_workbook(excel)
#对某表操作
    sheet=Book.sheet_by_name(sheetName)
#获取列数据
    #print(sheet.col_values(0))
    #print(sheet.cell_value(1,3))
#循环取数据
    #for i in range(sheet.ncols):
#获取各行数据
    list=[]
    id=1
    nrows = sheet.nrows #获取行数
    for i in range(id,nrows):
        body=sheet.cell(id,6).value.encode('utf-8')
        exp=sheet.cell(id,8).value
        list.append((body,exp))#字符串--字典
        id+=1
    #print(list)
    return list
if __name__ == '__main__':
    Getexcel('getcode')