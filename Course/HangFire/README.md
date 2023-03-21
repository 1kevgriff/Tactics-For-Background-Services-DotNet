# Tactics for Building Background Services: HangFire

## Example Connection String

If you're using our Dockerfile to run SQL Server, you can use the following connection string:

```
Server=localhost,1433;Database=HangFire;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=true;
```

## Overview - Why Background Services?

## Module: Introduction to HangFire 

## Module: Installation
done

## Module: Configuration
done

## Module: Setting up the Dashboard
done

## Module: Anatomy of a Job
done

## Module: On-demand Jobs
done

## Module: Handling Errors
done

## Module: Scheduled/Delayed Jobs
done

## Module: Recurring Jobs
done

## Module: Triggering Recurring Jobs
done

## Module: DI vs. Static Methods

## Module: Continuations

## Module: Job Servers

## Modules: Commonly changed HangFire settings
* Configuring the Number of Workers: Hangfire allows you to configure the number of worker threads that process jobs. Increasing the number of workers can help to reduce the time taken to process jobs, but it may also increase the load on the system.

* Setting the Queue Processing Timeout: Hangfire allows you to set a timeout value for the processing of a job in a queue. If the job processing time exceeds this value, the job is considered to have failed, and it will be re-queued for processing.

* Enabling Automatic Retry: Hangfire provides an automatic retry mechanism that can be configured to automatically retry failed jobs a certain number of times. This feature can help to improve the reliability of your application by automatically retrying failed jobs.

* Configuring the Dashboard: Hangfire provides a dashboard that displays information about the background jobs, such as the number of jobs processed, failed, or enqueued. You can configure the dashboard to display additional information or customize the dashboard UI to fit your needs.