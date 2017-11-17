//masud start
//To make a pivot in linq
//This will helper function
using System.Collections.Generic;
public DataTable ConvertGenericListToDatatable(List<object> lstProductID)
{
	
	
                lstProductID = lstProductID
                                .GroupBy(p => new { p.RawProductName })
                                .Select(s => new vmBOMSheet { 
                                   RawProductName = s.Key.RawProductName.ToString(),
                                   TotalPrice = s.Sum(a => a.TotalPrice),
                                   RecvQty = s.FirstOrDefault().RecvQty,
                                   ProductType = s.First().ProductType,
                                   lstChild = s.ToList()
                                }).ToList();
               
                DataTable dtProduct = new DataTable();

                dtProduct.Columns.Add("RawProductName");
                dtProduct.Columns.Add("ProductType");
                dtProduct.Columns.Add("RecvQty"); 
                foreach (var product in lstProductID)
                {
                    dtProduct.Rows.Add(new object[] { product.RawProductName, product.ProductType, product.RecvQty});
                }

                string[] uniqueQuarters = dtProduct.AsEnumerable().Select(x => x.Field<string>("ProductType")).Distinct().ToArray();

                var groups = dtProduct.AsEnumerable()
                            .GroupBy(x => x.Field<string>("RawProductName"))
                            .Select(x => x.GroupBy(y => y.Field<string>("ProductType"))
                                .Select(z => new
                                {
                                    rawProductName = x.Key,
                                    productType = z.Key,
                                    recvQty = z
                                        .Select((a, i) => new { recvQty = a.Field<string>("RecvQty"), index = i }).ToList()
                                }).ToList()).ToList();

                DataTable pivot = new DataTable();
                pivot.Columns.Add("RawProductName", typeof(string));
                pivot.Columns.Add("TotalPrice", typeof(string));
                foreach (string quarter in uniqueQuarters)
                {
                    pivot.Columns.Add(quarter, typeof(string));
                }

                foreach (var group in groups)
                {
                    int maxNewRows = group.Select(x => x.recvQty.Count()).Max();
                    for (int index = 0; index < maxNewRows; index++)
                    {
                        DataRow newRow = pivot.Rows.Add();
                        foreach (var row in group)
                        {
                            newRow["RawProductName"] = row.rawProductName;
                            newRow[row.productType] = row.recvQty.Skip(index) == null || !row.recvQty.Skip(index).Any() ? "0" : row.recvQty.Skip(index).First().recvQty;
                            newRow["TotalPrice"] = lstProductID.FirstOrDefault(a => a.RawProductName == row.rawProductName).TotalPrice;
                        }
                    }
                }
	return pivot;
                //masud end
}
//convert DataTable to generic List 
private static List<T> ConvertDataTable<T>(DataTable dt)  
{  
    List<T> data = new List<T>();  
    foreach (DataRow row in dt.Rows)  
    {  
        T item = GetItem<T>(row);  
        data.Add(item);  
    }  
    return data;  
}  
private static T GetItem<T>(DataRow dr)  
{  
    Type temp = typeof(T);  
    T obj = Activator.CreateInstance<T>();  
  
    foreach (DataColumn column in dr.Table.Columns)  
    {  
        foreach (PropertyInfo pro in temp.GetProperties())  
        {  
            if (pro.Name == column.ColumnName)  
                pro.SetValue(obj, dr[column.ColumnName], null);  
            else  
                continue;  
        }  
    }  
    return obj;  
}

//Cast List to Student type list

List< Student > studentDetails = new List< Student >();  
studentDetails = ConvertDataTable< Student >(dt);  