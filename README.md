# 🎬 MediaFlow

**MediaFlow** is a multithreaded C# console application for managing and processing media files through **FFmpeg**.

Media jobs are placed into a shared thread-safe queue and processed concurrently by a configurable number of worker threads.

### Supported Operations

* 🎞️ Media Conversion
* 🎵 Audio Extraction
* 📦 Media Compression

---

## ✨ Features

* Interactive console interface
* Thread-safe job queue
* Configurable worker count (`1–5`)
* Concurrent job processing
* Real-time progress tracking
* Live job monitor
* Individual job cancellation
* Cancel all unfinished jobs
* FFmpeg process management
* Job status tracking

Jobs can have the following states:

`Queued` · `Running` · `Completed` · `Failed` · `Canceled`

---

## ⚙️ How It Works

MediaFlow follows the **Producer-Consumer** model.

The user can add any number of jobs to a shared `BlockingCollection<MediaJob>`. Worker threads consume jobs from the queue and execute them through separate FFmpeg processes.

```text
                    MediaFlow
                        │
                        ▼
                  Console Menu
                        │
                        ▼
                   Job Manager
                        │
                        ▼
            BlockingCollection<MediaJob>
                        │
             ┌──────────┼──────────┐
             ▼          ▼          ▼
          Worker     Worker     Worker
             │          │          │
             ▼          ▼          ▼
           FFmpeg     FFmpeg     FFmpeg
```

The worker count is selected at runtime:

```text
Minimum Workers: 1
Maximum Workers: 5
```

For example, with **3 workers and 10 jobs**:

```text
Running: 3
Queued:  7
```

When a worker finishes, it automatically takes the next available job from the queue.

---


## 🖥️ Console Interface

The application will provide an interactive console interface similar to:

```text
========================================
               MEDIAFLOW
========================================

 Workers: 3 / 5
 Active:  2
 Queued:  4
 Done:    7

 [1] Add Job
 [2] List Jobs
 [3] Live Monitor
 [4] Cancel Job
 [5] Cancel All
 [6] Wait For All
 [7] Help
 [0] Exit

 Select >
```

The live monitor will display job status and progress while processing continues in the background.

```text
============================================================
                       LIVE MONITOR
============================================================

 #2  Compress
     movie.mp4
     [██████████████------] 67%     Running

 #3  Extract Audio
     podcast.mp4
     [██████--------------] 31%     Running

 #4  Convert
     trailer.mov
     [--------------------]  0%     Queued
```

---

## 🎞️ FFmpeg Integration

MediaFlow works directly with external FFmpeg processes using:

* `System.Diagnostics.Process`
* `ProcessStartInfo`
* Standard output/error redirection
* Exit codes
* Process termination
* Progress parsing

Each media operation defines its FFmpeg arguments, while a shared process runner handles execution.

```text
JobWorker
    │
    ▼
MediaProcessor
    │
    ▼
FfmpegProcessRunner
    │
    ▼
FFmpeg
```

---

## 🏗️ Project Structure

```text
MediaFlow
│
├── Program.cs
│
├── Models
│   ├── MediaJob.cs
│   ├── JobStatus.cs
│   └── MediaOperationType.cs
│
├── Jobs
│   └── JobManager.cs
│
├── Workers
│   └── JobWorker.cs
│
├── Processing
│   ├── IMediaProcessor.cs
│   ├── MediaProcessorResolver.cs
│   ├── MediaConverter.cs
│   ├── AudioExtractor.cs
│   └── MediaCompressor.cs
│
├── Processes
│   ├── FfmpegProcessRunner.cs
│   └── FfmpegProgressParser.cs
│
├── UI
│   ├── ConsoleMenu.cs
│   ├── JobMonitor.cs
│   └── HelpScreen.cs
│
└── Configuration
    └── AppSettings.cs
```

---

## 🛠️ Technologies

`C#` · `.NET` · `FFmpeg` · `Thread` · `Process` · `BlockingCollection<T>` · `Synchronization` · `Producer-Consumer`

---

## 👥 Team

