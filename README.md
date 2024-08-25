# SplitScreen - Cemu Screen Splitter

SplitScreen is a tool designed to split the display of an application with a split-screen rendering into two separate windows. Originally developed for the local multiplayer mod of *The Legend of Zelda: Breath of the Wild* on Cemu, this program allows you to enhance your gaming experience by providing more flexibility in screen management.

## Features

- Split the screen of any software that uses split-screen rendering into two separate windows.
- Customize the aspect ratio and border cropping to ensure the best possible display.
- Press F11 to put a window in full screen

## Usage

To launch the program, use the following syntax:

```bash
.\SplitScreen.exe <aspect ratio X:x> <new borders (left, top, right, bottom)>
```

### Example:

```bash
.\SplitScreen.exe 16:9 0,50,0,75
```

If the program is launched without any arguments, it will use the following default settings:

```bash
.\SplitScreen.exe 16:9 0,0,0,0
```

### Customization Options:

- **Aspect Ratio:** You can modify the aspect ratio of the split screens. Be sure to follow the syntax as shown above.
- **Borders:** If the split windows are not perfectly aligned, you can redefine the borders to crop them properly.

## Example Usage for Zelda BOTW Multiplayer on Cemu

You have two options for setting up the split screen:

### Option 1:

1. Set the fullscreen scaling to **Stretch**.
2. Set Cemu to **windowed mode**.
3. Set the game to **16:9** (for this example).
4. Define the borders to crop the menu bar:
   ```bash
   .\SplitScreen.exe 16:9 0,50,0,75
   ```
5. Select the Cemu window.

### Option 2:

1. Set the fullscreen scaling to **Stretch**.
2. Set Cemu to **fullscreen mode**.
3. Set the game to **16:9** (for this example).
4. Simply run:
   ```bash
   .\SplitScreen.exe
   ```
5. Select the Cemu window.

## Notes

- The program was developed in just two days and was originally created for personal use. While it works as intended, it may not cover all use cases.
