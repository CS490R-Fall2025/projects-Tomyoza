[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/Rs_C30LP)
# Class Template – Unity WebGL Publishing

## What goes in this repo?
- **Do not** commit your whole Unity project here (keeps the repo small).
- **Only** commit your **WebGL build outputs** into:
  - `docs/project1/` for Project 1
  - `docs/project2/` for Project 2

> The repo already has a `.gitignore` that excludes big Unity folders (e.g., `Library/`, `Temp/`, etc.).

---

## How to publish your WebGL build

1. **In Unity**:  
   - `File -> Build Settings...`  
   - Select **WebGL** as the platform → click **Switch Platform** (if needed).  
   - Click **Build** (not Build & Run) and choose a temporary folder on your machine.

   Unity will export **three things**:
   - `index.html`
   - a `Build/` folder
   - a `TemplateData/` (or similarly named) folder

2. **Copy to this repo**:  
   - For **Project 1**: copy the exported `index.html`, `Build/`, and `TemplateData/` into `docs/project1/`.  
   - For **Project 2**: do the same into `docs/project2/`.

3. **Commit & push**:
   ```bash
   git add docs/
   git commit -m "Add WebGL build"
   git push
