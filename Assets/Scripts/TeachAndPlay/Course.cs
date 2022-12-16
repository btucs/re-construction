using System;
using UnityEngine;
using Newtonsoft.Json;

public class Course 
{
	public string name;
	public string accessCode;
	public bool accessable; //status
	[Obsolete]
	public DateTime joinDate;

	public CourseTask[] tasks;
	public Teacher[] teachers;
	public Student[] students;

	public CourseTask GetNextTask(GameState saveData)
	{
		for(int i=0; i<tasks.Length; i++)
		{
			int tasksDone = saveData.taskHistoryData.GetCourseTaskAmount(tasks[i].topic);
			

			//Debug.Log("Required amount for task is: " + tasks[i].taskAmount);
			int tasksRequired = tasks[i].taskAmount;
			tasksRequired = Mathf.Clamp(tasksRequired, 1, 3);

			Debug.Log("For Topic " + tasks[i].topic.name + " there are " + tasks[i].taskAmount + " tasks to be done. Currently " + tasksDone + " tasks have been completed.");


			if(tasksDone < tasksRequired)
			{
				return tasks[i];
			} else {
				Debug.Log("Topic has been completed: " + tasks[i].topic.name + " so now checking topic at index after: " + i);
			}
		}
		return null;
	}

	public int GetNumberOfTasks()
	{
		int taskNr = 0;
		for(int i = 0; i<tasks.Length; i++)
		{
			int tasksRequired = tasks[i].taskAmount;
			tasksRequired = Mathf.Clamp(tasksRequired, 1, 3);
			
			taskNr += tasksRequired;
		}
		return taskNr;
	}

	public int TopicsFinished(GameState saveData)
	{
		int finished = 0;
		for(int i=0; i<tasks.Length; i++)
		{
			if(saveData.taskHistoryData.GetCourseTaskAmount(tasks[i].topic)>=tasks[i].taskAmount)
				finished ++;
		}
		return finished;
	}

	
}

public class CourseTask
{
	[JsonConverter(typeof(NewtonSoftJSONTopicSOConverter))]
	public TopicSO topic;
	public DateTime accessibleFrom;
	public DateTime accessibleUntil;
	public MLEQuiz[] singleChoiceQuestions;
	public int taskAmount;

	public bool IsDateValid()
	{
		DateTime currentDate = DateTime.Now;
		int compareStart = DateTime.Compare(accessibleFrom, currentDate);
		int compareEnd = DateTime.Compare(accessibleUntil, currentDate);
		
		return (compareStart < 0 && compareEnd > 0);
	}

	public string GetAsString()
	{
		string returnVal = "Aufgabenthema: " + topic.ToString() + ". Aufgaben: " + taskAmount;
		return returnVal;
	}

	public CourseTask (TopicSO _topic, int _taskAmount, DateTime _from, DateTime _until)
	{
		topic = _topic;
		taskAmount = _taskAmount;
		accessibleFrom = _from;
		accessibleUntil = _until;
	}

	public bool HasBeenCompleted(GameState saveData)
	{
		return (saveData.taskHistoryData.GetCourseTaskAmount(this.topic) >= taskAmount);
	}

	public bool Compare(CourseTask compareVal)
	{
		return (DateTime.Compare(compareVal.accessibleFrom, this.accessibleFrom) == 0 &&
			DateTime.Compare(compareVal.accessibleUntil, this.accessibleUntil) == 0 &&
			compareVal.topic.name == this.topic.name);
	}
}

public class Teacher
{
	public string name;
	public string email;
}

public class Student
{
	public string userName;
	public string userID;
}