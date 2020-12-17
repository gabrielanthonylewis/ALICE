public class AssaultRifle : Gun
{
    // Change the Fire Type (Fully Automatic, Burst and Single shot).
    public override void OnChangeFireTypeInput()
    {
        base.OnChangeFireTypeInput();

        if(this.isFiring)
            return;
        
        this.audioSource.PlayOneShot(this.fireTypeSound);
        this.NextFireType();
    }
}
