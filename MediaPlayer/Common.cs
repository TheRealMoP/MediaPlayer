using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace MediaPlayer
{
    public static class Common
    {
        public static async Task<StorageFile> OpenFile(PickerLocationId locationId, string[] fileExtensions)
        {
            FileOpenPicker openPicker = new FileOpenPicker()
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = locationId
            };

            foreach (var ext in fileExtensions)
            {
                openPicker.FileTypeFilter.Add(ext);
            }
            
            var file = await openPicker.PickSingleFileAsync();
            
            return file;
        }

        public static async Task<IReadOnlyList<StorageFile>> OpenFileMulti(PickerLocationId locationId, string[] fileExtensions)
        {
            FileOpenPicker openPicker = new FileOpenPicker()
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = locationId
            };

            foreach (var ext in fileExtensions)
            {
                openPicker.FileTypeFilter.Add(ext);
            }

            var files = await openPicker.PickMultipleFilesAsync();
            
            return files;
        }

        public static async Task<StorageFolder> OpenFolder(PickerLocationId locationId, string[] fileExtensions)
        {
            FolderPicker folderPicker = new FolderPicker()
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = locationId
            };
            
            var folder = await folderPicker.PickSingleFolderAsync();
            
            return folder;
        }
    }
}
