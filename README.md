# The interior of the amygdala 
A Virtual Reality experience built in Unity, showcasing interactive environments, AI-driven chatbots, emotion detection, and more. 
This project is versioned with Git and uses Git Large File Storage (LFS) for large assets.

# Table of Contents
- [Features](#features)
- [Prerequisites](#prerequisites)
- [Project Setup](#project-setup)
- [Usage](#usage)
- [In‑Editor Testing](#in-editor-testing)
- [Building for a VR Headset (Oculus Quest 3S)](#building-for-a-vr-headset-oculus-quest-3s)
- [Folder Structure](#folder-structure)
- [Contributing](#contributing)
- [License](#license)

# Features
- AI‑driven chatbot
- Emotion detection via Hugging Face APIs
- MIDI‑driven audio playback and guitar simulation
- Interactive environment with draggable objects and a mini‑map

# Prerequisites
- Unity 2021.3 LTS or later
- Git 2.28+
- Git LFS
- Android SDK & NDK (for Quest builds)
- Oculus XR Plugin via Unity Package Manager
- XR Interaction Toolkit and Input System packages via Unity Package Manager

# Project Setup
 - Clone the repo: git clone https://github.com/1gabriella/VR-Nostalgia-Final.git
- cd VR-Nostalgia-Final
- git lfs install
- git lfs pull

Open VR-Nostalgia-Final in Unity.

Install the Oculus XR Plugin:
In Window → Package Manager, switch to Unity Registry.
Search for Oculus XR Plugin and click Install.

<img width="271" alt="Screenshot 2025-04-20 at 14 49 43" src="https://github.com/user-attachments/assets/7755f333-55d8-4c45-a09d-226d849e800d" /> 
<img width="819" alt="Screenshot 2025-04-20 at 14 53 55" src="https://github.com/user-attachments/assets/4677533a-7324-4ae5-94f2-e92ebc53d82f" />

Configure XR Management:
In Edit → Project Settings → XR Plug-in Management, enable Oculus under Android.
<img width="884" alt="Screenshot 2025-04-20 at 14 51 29" src="https://github.com/user-attachments/assets/ef6e7d68-ef27-4e18-97dd-93cad185c4d8" /> 

# Usage 

In Unity Editor, press Play to test in the Editor (note there is no player so to walk around scene only compatible with VR)
Building for a VR Headset (Oculus Quest 3S)
Set the build target to Android:
File → Build Settings, select Android, then click Switch Platform.
<img width="819" alt="Screenshot 2025-04-20 at 14 58 03" src="https://github.com/user-attachments/assets/962b9091-6d9e-4de7-a064-29097871c52f" />

Add scene to build  →  build settings  →  Add open scenes  → 

Add head set → build settings  → Run Devide 
<img width="711" alt="Screenshot 2025-04-20 at 15 02 23" src="https://github.com/user-attachments/assets/16150dcc-1b94-4f92-b4a5-715ff21aa9bd" /> 

Build & Run:
Connect your headset via USB (enable Developer Mode).
Click Build and Run. Unity will compile and deploy the APK to the headset.

Put on your headset and locate the app under Unknown Sources.

# Usage 
Fork the repo.
Create a feature branch.
Commit changes with clear messages.
Open a Pull/Merge Request.

