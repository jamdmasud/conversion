
select 
*
from
	(
		select
			rm.OrderID, 
			rp.Name as TypeName, 
			ISNULL(rm.QTY, 0) Quantity 
		from G_RequiredMaterial rm
		left join G_RawProductType rp on rm.RawProductTypeID = rp.RawProductTypeID
	) 
AS SourceTable
PIVOT
	(
		SUM(Quantity)
		FOR TypeName in ([Care Label], Carton, Elastic,EMBO, Fabric, [Hang Tag], [Main Label],[Swing Thread],Yarn)

	)
PivotTable
