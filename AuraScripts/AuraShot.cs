public class AuraShot
{
    public static string Execute()
    {
        if (!UnityEngine.Application.isPlaying) return "NO_PLAY";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        UnityEngine.GameObject player = UnityEngine.GameObject.Find("Player");
        if (player == null) return "NO_PLAYER";

        UnityEngine.MonoBehaviour view = null;
        UnityEngine.MonoBehaviour[] comps = player.GetComponents<UnityEngine.MonoBehaviour>();
        for (int i = 0; i < comps.Length; i++)
        {
            if (comps[i] != null && comps[i].GetType().Name == "SurveillanceView") view = comps[i];
        }
        if (view == null) return "NO_VIEW";

        System.Type t = view.GetType();
        System.Reflection.PropertyInfo pActive = t.GetProperty("IsActive");
        sb.Append("IsActive=").Append(pActive != null ? pActive.GetValue(view, null).ToString() : "?");

        UnityEngine.GameObject camGo = UnityEngine.GameObject.Find("CamaraVigilanciaFeed");
        if (camGo != null)
        {
            UnityEngine.Camera cam = camGo.GetComponent<UnityEngine.Camera>();
            sb.Append(" feedCam=").Append(cam != null && cam.enabled);
            sb.Append(" rt=").Append(cam != null && cam.targetTexture != null ? cam.targetTexture.name : "null");
            sb.Append(" fov=").Append(cam != null ? cam.fieldOfView.ToString("F1") : "?");
            sb.Append(" pos=").Append(camGo.transform.position.ToString());
        }
        else sb.Append(" feedCam=null");

        UnityEngine.GameObject canvasGo = UnityEngine.GameObject.Find("VigilanciaUI");
        if (canvasGo != null)
        {
            UnityEngine.Canvas cv = canvasGo.GetComponent<UnityEngine.Canvas>();
            sb.Append(" canvas=").Append(cv != null ? cv.sortingOrder.ToString() : "?");
            sb.Append(" canvasActive=").Append(canvasGo.activeSelf);
            UnityEngine.UI.RawImage[] imgs = canvasGo.GetComponentsInChildren<UnityEngine.UI.RawImage>(true);
            for (int i = 0; i < imgs.Length; i++)
            {
                UnityEngine.RenderTexture rt = imgs[i].texture as UnityEngine.RenderTexture;
                string texName = rt != null ? rt.name : (imgs[i].texture != null ? imgs[i].texture.name : "null");
                sb.Append(" | img").Append(i).Append(" ").Append(imgs[i].gameObject.name)
                  .Append(" tex=").Append(texName)
                  .Append(" w=").Append(imgs[i].rectTransform.rect.width.ToString("F0"))
                  .Append(" h=").Append(imgs[i].rectTransform.rect.height.ToString("F0"));
            }
        }
        else sb.Append(" canvas=null");

        UnityEngine.GameObject hud = UnityEngine.GameObject.Find("HUD");
        if (hud != null)
        {
            UnityEngine.Canvas hc = hud.GetComponent<UnityEngine.Canvas>();
            sb.Append(" HUD=").Append(hc != null ? hc.sortingOrder.ToString() : "?");
        }

        string dir = "C:/Users/Chris/Heartbeat/AuraCaptures";
        System.IO.Directory.CreateDirectory(dir);
        string file = dir + "/vigilancia.png";
        if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
        UnityEngine.ScreenCapture.CaptureScreenshot(file);
        sb.Append(" shot=").Append(file);

        return sb.ToString();
    }
}
