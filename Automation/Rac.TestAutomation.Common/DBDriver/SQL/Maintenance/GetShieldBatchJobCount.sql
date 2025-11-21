/*
PURPOSE: Search for jobs from the list in batchjoblist.json 
		 Returns the trigger_state as we need to investigate PAUSED jobs as they will
		 not do anything useful. 
		 We can't just assume the job needs scheduling though, too likely we will end
		 up with multiple jobs scheduled that way.
AUTHOR:  Jason King
LAST UPDATE: Troy Hall 2021-05-26
*/
select 
		qjd.job_name 
		,qt.trigger_state
  from 
		qrtz_job_details qjd
		join qrtz_triggers qt on qt.sched_name = qjd.sched_name and qt.job_name = qjd.job_name and qt.job_group = qjd.job_group
where lower(qjd.job_name) like @JobName
  and lower(qjd.job_name) not like '%monitoring%'
;