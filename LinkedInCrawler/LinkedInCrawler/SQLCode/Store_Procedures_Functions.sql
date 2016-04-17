
/****** Object:  UserDefinedFunction [dbo].[ufn_SplitString]    Script Date: 4/17/2016 1:59:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
Create FUNCTION [dbo].[ufn_SplitString]
    (
        @List NVARCHAR(MAX),
        @Delim VARCHAR(255)
    )
    RETURNS TABLE
    AS
        RETURN ( SELECT [Value] FROM 
          ( 
            SELECT 
              [Value] = LTRIM(RTRIM(SUBSTRING(@List, [Number],
              CHARINDEX(@Delim, @List + @Delim, [Number]) - [Number])))
            FROM (SELECT Number = ROW_NUMBER() OVER (ORDER BY name)
              FROM sys.all_objects) AS x
              WHERE Number <= LEN(@List)
              AND SUBSTRING(@Delim + @List, [Number], LEN(@Delim)) = @Delim
          ) AS y
        );

GO
/****** Object:  StoredProcedure [dbo].[Add_User]    Script Date: 4/17/2016 1:59:55 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Nadav Stern
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Add_User]
	-- Add the parameters for the stored procedure here
	@Name nvarchar(50),
	@Title nvarchar(50),
	@CurrentPosition nvarchar(50),
	@Skills nvarchar(max),
	@TopSkillsCounter int,
	@Summary text 
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	DECLARE @UserID as  int = 0;
	CREATE TABLE #tempTBL (Name nvarchar(50));

	SELECT  @UserID =  ID  FROM Users
	WHERE Name = @Name AND Title = @Title AND  Current_Position = @CurrentPosition

	IF @UserID <> 0
		BEGIN
			UPDATE Users
			SET Top_Skills_Counter = @TopSkillsCounter
			WHERE ID = @UserID	
		END
	ELSE
		BEGIN
			INSERT INTO Users (Name, Title, Current_Position, Summary, Top_Skills_Counter)
			values (@Name,@Title,@CurrentPosition,@Summary,@TopSkillsCounter)

			SELECT @UserID = MAX(ID) FROM Users 
		END
	
	INSERT INTO #tempTBL
	SELECT * FROM [dbo].[ufn_SplitString] (@Skills,',')

	INSERT INTO Skills(name)
	SELECT NAME from #tempTBL
	WHERE  NAME not in (SELECT name from Skills)
	
	DELETE UsersSkills
	WHERE User_ID = @UserID

	INSERT INTO UsersSkills (User_ID,Skill_ID)
	SELECT @UserID, ID	
	FROM Skills SK JOIN #tempTBL TMP on SK.name = TMP.Name
	
END

GO
/****** Object:  StoredProcedure [dbo].[Get_Skills]    Script Date: 4/17/2016 1:59:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Nadav Stern
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Get_Skills]
	-- Add the parameters for the stored procedure here
	@UserID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SELECT Skills.name 
	FROM Skills INNER JOIN UsersSkills ON Skills.ID = UsersSkills.Skill_ID
	WHERE UsersSkills.User_ID = @UserID;

END


GO
/****** Object:  StoredProcedure [dbo].[Get_User]    Script Date: 4/17/2016 1:59:56 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Nadav Stern
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Get_User]
	-- Add the parameters for the stored procedure here
	@Name nvarchar(50)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SELECT * 
	FROM Users 
	Where Name = @Name
END
