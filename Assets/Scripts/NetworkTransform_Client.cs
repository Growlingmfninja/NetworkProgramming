using Unity.Netcode.Components;

public class NetworkTransform_Client : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
