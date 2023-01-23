﻿CREATE VIEW [dbo].[WP_COUNT_DRILLING_OPERATIONS_CATEGORIES_OF_WELLS]
AS
SELECT     COUNT(Category) AS COUNT, Category, Year_of_WP
FROM         dbo.DRILLING_OPERATIONS_CATEGORIES_OF_WELLS
GROUP BY Year_of_WP, Category