using UnityEngine;

namespace BerserkPixel.Readme
{
    public class Shortcuts
    {
        private readonly string _assetName;
        private readonly BerserkURL _urls;

        public Shortcuts(string assetName, BerserkURL urls)
        {
            _assetName = assetName;
            _urls = urls;
        }

        public void OpenReviewsPage()
        {
            Application.OpenURL(_urls.URL_REVIEWS);
        }

        public void ShowOnlineManual()
        {
            Application.OpenURL(_urls.URL_DOCS);
        }

        public void ShowYoutube()
        {
            Application.OpenURL(_urls.URL_YOUTUBE);
        }

        public void OpenBerserkStorePage()
        {
            Application.OpenURL(_urls.URL_STORE_PAGE);
        }

        public void ShowSupportEmailEditor()
        {
            OpenEmailEditor(
                _urls.URL_SUPPORT_EMAIL,
                $"[{_assetName}] SHORT_QUESTION_HERE",
                "YOUR_QUESTION_IN_DETAIL");
        }

        public void ShowSupportEmailEditor(string message = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
                OpenEmailEditor(
                    _urls.URL_SUPPORT_EMAIL,
                    $"[{_assetName}] SHORT_QUESTION_HERE",
                    message);
            else
                ShowSupportEmailEditor();
        }

        public void ShowBusinessEmailEditor()
        {
            OpenEmailEditor(
                _urls.URL_BUSINESS_EMAIL,
                $"[{_assetName}] SHORT_QUESTION_HERE",
                "YOUR_QUESTION_IN_DETAIL");
        }

        private void OpenEmailEditor(string receiver, string subject, string body)
        {
            var url = $"mailto:{receiver}" + $"?subject={subject.Replace(" ", "%20")}" +
                      $"&body={body.Replace(" ", "%20")}";

            Application.OpenURL(url);
        }
    }
}