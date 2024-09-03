# SplitScreen

**SplitScreen** is a tool designed to split the display of an application using a split-screen rendering into multiple distinct windows.

## Features

- Split the screen of any software using split-screen rendering into multiple distinct windows.
- Customize the aspect ratio and crop borders to ensure the best possible display.
- Press `F11` to switch any window to fullscreen mode.

## Usage

To launch the program, use the following syntax in a terminal:

```bash
.\SplitScreen.exe <json file config>
```

**Example:**

```bash
.\SplitScreen.exe "C:\Users\public\Cemu\config.json"
```

## JSON Configuration File Explanation

The configuration file is a crucial part of how SplitScreen works. The number of windows created is determined by the number of configurations specified in the `screenConfigs` array. Each window configuration has three main attributes: `aspect_type`, `capture_type`, and `border`. Depending on the choices made within these attributes, additional parameters will be required.

### aspect_type

This attribute manages the aspect ratio of the duplicated window.

- **If `aspect_type` is set to `"ratio"`**  
  A supplementary parameter named `ratio` is required. This parameter allows you to specify the aspect ratio using a format like `x:x` (e.g., `16:9`). This ratio will be used to scale the window's content accordingly.

  **JSON Example:**

  ```json
  {
    "screenConfigs": [
      {
        "aspect_type": "ratio",
        "ratio": "16:9",
        ...
      }
    ]
  }
  ```

- **If `aspect_type` is set to `"size"`**  
  A supplementary parameter named `size` is required. This parameter allows you to define the window's dimensions using raw values in the format `Width x Height` (e.g., `300x300`).

  **JSON Example:**

  ```json
  {
    "screenConfigs": [
      {
        "aspect_type": "size",
        "size": "300x300",
        ...
      }
    ]
  }
  ```

### capture_type

This attribute controls how the image from the application is captured.

- **If `capture_type` is set to `"sliced"`**  
  Additional parameters `rows`, `columns`, and `screen_index` are required. This setup allows you to proportionally slice the image into a grid and select the desired portion. For instance, slicing into 2 rows and 3 columns and selecting the first row in the second column (`screen_index` starts at 0).
  
  - **How It Works:** 
  When you slice the screen into a grid using the `rows` and `columns` parameters, the screen is divided into smaller sections. Each section is assigned an index, starting from `0`. The `screen_index` parameter specifies which section of this grid you want to display in the window.

  - **Indexing:** 
  The sections are indexed in a row-major order, meaning the indexing starts at the top-left corner of the screen and proceeds horizontally across the first row before moving to the next row. For example:
  
  - If you have `2 rows` and `3 columns`, the screen will be divided into 6 segments with the following indices:

    | Index | Position              |
    |-------|-----------------------|
    | 0     | Top-left (Row 1, Col 1)|
    | 1     | Top-center (Row 1, Col 2)|
    | 2     | Top-right (Row 1, Col 3)|
    | 3     | Bottom-left (Row 2, Col 1)|
    | 4     | Bottom-center (Row 2, Col 2)|
    | 5     | Bottom-right (Row 2, Col 3)|

  - **Example Usage:**
  If you want to capture the section in the top-center of a 2x3 grid (which corresponds to index `1`), you would set `screen_index` to `1`. The JSON configuration would look like this:


  **JSON Example:**

  ```json
  {
    "screenConfigs": [
      ...
      "capture_type": "sliced",
      "rows": 2,
      "columns": 2,
      "screen_index": 1,
      ...
    ]
  }
  ```

- **If `capture_type` is set to `"cropped"`**  
  An additional parameter named `coordinates` is required. This parameter allows you to specify a custom rectangular area of the screen to capture using the format `[x, y, width, height]`. This method is particularly useful when you need to precisely define the area of interest on the screen.

  **JSON Example:**

  ```json
  {
    "screenConfigs": [
      ...
      "capture_type": "cropped",
      "coordinates": [0, 0, 800, 800],
      ...
    ]
  }
  ```

- **If `capture_type` is set to `"full"`**  
  No additional parameters are needed, as this option captures the entire screen.

### border

This attribute is layered on top of `capture_type` and allows you to redefine the borders to eliminate unwanted elements. Border values can be negative, allowing for fine-tuned cropping.

**JSON Example:**

```json
{
  "screenConfigs": [
    ...
    "border": {
      "left": 0,
      "top": 0,
      "right": 0,
      "bottom": 0
    }
  ]
}
```

### Example of a Valid JSON Configuration

```json
{
  "screenConfigs": [
    {
      "aspect_type": "ratio",
      "ratio": "16:9",
      "capture_type": "sliced",
      "rows": 2,
      "columns": 2,
      "screen_index": 1,
      "border": {
        "left": 0,
        "top": 0,
        "right": 0,
        "bottom": 0
      }
    },
    {
      "aspect_type": "ratio",
      "ratio": "21:9",
      "capture_type": "cropped",
      "coordinates": [0, 0, 800, 800],
      "border": {
        "left": -10,
        "top": 100,
        "right": 50,
        "bottom": -20
      }
    },
    {
      "aspect_type": "size",
      "size": "300x300",
      "capture_type": "full",
      "border": {
        "left": 0,
        "top": 0,
        "right": 100,
        "bottom": 100
      }
    }
  ]
}
```

## Usage Example

**Step 1: Create Your JSON Configuration File**

First, create your JSON file with the appropriate configuration (e.g., "config.json").

**Step 2: Launch SplitScreen**

Open a terminal in the SplitScreen directory and run the program with the path to your JSON file:

```bash
.\SplitScreen.exe "C:\Users\public\Cemu\config.json"
```

**Step 3: Select the Application to Duplicate**

After running the command, choose the application you want to duplicate.

And that's it! You can also create a `.bat` file to automate this process. Here's how:

1. Open a text editor and paste the following content:

   ```bash
   @echo off
   cd path_to_SplitScreen_directory
   SplitScreen.exe "path_to_your_json\config.json"
   ```

2. Save the file with a `.bat` extension (e.g., `launchSplitScreen.bat`).

3. Double-click the `.bat` file to run SplitScreen with your configuration.

## Notes

This program was developed in just two days and was originally created for personal use. While it functions as intended, it may not cover all use cases.
