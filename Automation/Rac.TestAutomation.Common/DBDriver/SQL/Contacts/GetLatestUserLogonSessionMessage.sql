/*
PURPOSE: Find the latest log in/out message in the past
         hour for a specific username.
         Relies on "usernameRegex" to essentially be
         defining a "ends with" type syntax, e.g.:
         "%AutoMation1" would mean ends-with 'AutoMation1'
AUTHOR:  Jason King
LAST UPDATE: Jason King 2021-06-18
*/
SELECT top 1 stu.id, stu.LOG_MESSAGE, stu.UPDATE_DATE 
  FROM st_user_mng_log stu 
 WHERE stu.LOG_MESSAGE like @usernameRegex 
   AND stu.LOG_MESSAGE like '%logged%' 
   AND stu.UPDATE_DATE > DATEADD(MINUTE, -3, GETDATE()) 
ORDER BY stu.UPDATE_DATE desc;