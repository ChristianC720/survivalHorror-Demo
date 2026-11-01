public class VigState
{
    public static string Execute()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        UnityEngine.GameObject player = UnityEngine.GameObject.Find("Player");
        Heartbeat.Player.SurveillanceView view = player.GetComponent<Heartbeat.Player.SurveillanceView>();

        System.Type t = typeof(Heartbeat.Player.SurveillanceView);
        System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;

        UnityEngine.Camera feedCamera = t.GetField("feedCamera", flags).GetValue(view) as UnityEngine.Camera;
        object buffersObj = t.GetField("feedBuffers", flags).GetValue(view);
        System.Array buffers = buffersObj as System.Array;
        int writeIndex = (int)t.GetField("writeIndex", flags).GetValue(view);
        int writtenSamples = (int)t.GetField("writtenSamples", flags).GetValue(view);
        UnityEngine.UI.RawImage feedImage = t.GetField("feedImage", flags).GetValue(view) as UnityEngine.UI.RawImage;
        UnityEngine.UI.RawImage noiseImage = t.GetField("noiseImage", flags).GetValue(view) as UnityEngine.UI.RawImage;
        UnityEngine.Canvas canvas = t.GetField("feedCanvas", flags).GetValue(view) as UnityEngine.Canvas;
        UnityEngine.Camera viewCamera = t.GetField("viewCamera", flags).GetValue(view) as UnityEngine.Camera;

        sb.AppendLine("[Aura] activa=" + view.IsActive + " bloqueada=" + view.IsBlocked
            + " señal=" + view.Signal + " ruido=" + view.NoiseAmount + " retardo=" + view.FeedDelay);

        sb.AppendLine("[Aura] camara OTS enabled=" + (viewCamera != null ? viewCamera.enabled.ToString() : "null"));
        sb.AppendLine("[Aura] feed camera=" + (feedCamera != null ? feedCamera.name + " enabled=" + feedCamera.enabled : "null")
            + " fov=" + (feedCamera != null ? feedCamera.fieldOfView : 0f)
            + " pos=" + (feedCamera != null ? feedCamera.transform.position.ToString() : "-")
            + " rot=" + (feedCamera != null ? feedCamera.transform.eulerAngles.ToString() : "-"));

        sb.AppendLine("[Aura] buffer: " + (buffers != null ? buffers.Length : 0) + " RTs | writeIndex=" + writeIndex
            + " escritos=" + writtenSamples);
        sb.AppendLine("[Aura] targetTexture=" + (feedCamera != null && feedCamera.targetTexture != null ? feedCamera.targetTexture.name : "null"));

        int delayFrames = (int)System.Math.Round(view.FeedDelay * 20f);
        sb.AppendLine("[Aura] retardo en fotogramas=" + delayFrames
            + " | indice leido=" + (feedImage != null && feedImage.texture != null ? feedImage.texture.name : "null"));

        sb.AppendLine("[Aura] canvas=" + (canvas != null ? canvas.name + " activo=" + canvas.gameObject.activeSelf : "null")
            + " | ruido alpha=" + (noiseImage != null ? noiseImage.color.a.ToString("F3") : "-"));

        Heartbeat.AI.EnemyDirector director = Heartbeat.AI.EnemyDirector.Instance;
        sb.AppendLine("[Aura] The Mother=" + (director != null ? director.MotherAttention01.ToString("F3") : "-"));

        return sb.ToString();
    }
}
