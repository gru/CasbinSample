using Casbin;

namespace CasbinSample;

public static class Program
{
    public static void Main()
    {
        var enforcer = new Enforcer("model.conf", "policy.csv");
        
        // Одинаковые роли имеют одинаковые разрешения в разных филиалах (branch)
        
        // Alice supervisor везде
        // Права без указания филиала
        Console.WriteLine(enforcer.Enforce("alice", "account", "read"));
        // Права с указанием филиала
        var rbacContext = enforcer.CreateContext(requestType: "r2", matcherType: "m2"); 
        Console.WriteLine(enforcer.Enforce(rbacContext, "alice", "branch1", "account", "read"));
        
        // Bob business_user в branch1
        // Не имеет прав без указания филиала
        Console.WriteLine(enforcer.Enforce("bob", "account", "read"));
        // Имеет права на чтение account в branch1
        Console.WriteLine(enforcer.Enforce(rbacContext, "bob", "branch1", "account", "read"));
        
        // ABAC
        // Alice supervisor может читать любые документы в любом филиале
        var letter = new Document(1, "letter");
        var news = new Document(2, "news");

        var abacContext = enforcer.CreateContext(requestType: "r3", matcherType: "m3", policyType: "p2");
        Console.WriteLine(enforcer.Enforce(abacContext, "alice", "any", letter, "read"));
        Console.WriteLine(enforcer.Enforce(abacContext, "alice", "branch1", news, "read"));
        
        // Bob может читать письма (letter) в своем филиале
        Console.WriteLine(enforcer.Enforce(abacContext, "bob", "branch1", letter, "read"));
        Console.WriteLine(enforcer.Enforce(abacContext, "bob", "branch1", news, "read"));
        
        // Bob не может читать письма в чужом филиале
        Console.WriteLine(enforcer.Enforce(abacContext, "bob", "any", letter, "read"));
    }
}

public record Document(int Id, string DocumentType);
