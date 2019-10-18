public interface Pausible
{
    void Pause();
    void Unpause();
    void privOnAwakeRegisterPausible();
    void privOnDestroyRegisterPausible();
}