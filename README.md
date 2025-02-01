# JetBrains License Dropper  

A tool for completely removing all JetBrains product settings. All actions, both successful and unsuccessful, are reported.  

## Features:  
- Clears temporary files (**Temp** folder).  
- Deletes settings folders in **Roaming** and **Local**.  
- Removes **registry keys** related to JetBrains settings.  
- Optionally removes **Java-JetBrains** dependent registry keys (recommended if previous steps did not help).  

## Preserved Data (for Rider 2021.1):  
The following settings will be **retained**:  
- **Live Templates**  
- **Keymap** (currently only with VS-copy)  
- **Recent Solutions**  
- **Trusted Solutions**  
- **External Tools**  

## Usage:  
During the cleanup process, you will be prompted to confirm each step:  
- **"yes"** → Execute the cleaning step.  
- **"no"** → Skip the cleaning step.  
