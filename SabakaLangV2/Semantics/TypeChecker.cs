using SabakaLangV2.AST;
using SabakaLangV2.AST.Expressions;

namespace SabakaLangV2.Semantics;

public class TypeChecker
{
    private SymbolTable _scope = new();
    private TypeSymbol _currentReturnType = BuiltinTypeSymbol.Void;

    public void Check(List<Statement> statements)
    {
        foreach (var stmt in statements)
            CheckStatement(stmt);
    }

    private void CheckStatement(Statement stmt)
    {
        switch (stmt)
        {
            case VariableDeclaration v:
                CheckVariableDeclaration(v);
                break;

            case ExpressionStatement e:
                CheckExpression(e.Expression);
                break;

            case ReturnStatement r:
                CheckReturn(r);
                break;

            case BlockStatement b:
                EnterScope();
                foreach (var s in b.Statements)
                    CheckStatement(s);
                ExitScope();
                break;
            
            case FunctionDeclaration f:
                CheckFunction(f);
                break;
        }
    }

    private void CheckVariableDeclaration(VariableDeclaration v)
    {
        var type = ResolveType(v.TypeName);

        if (!_scope.Declare(v.Name, type))
            throw new Exception($"Variable '{v.Name}' already declared");

        if (v.Initializer != null)
        {
            var initType = CheckExpression(v.Initializer);

            if (initType != type)
                throw new Exception($"Cannot assign {initType} to {type}");
        }
    }

    private TypeSymbol CheckExpression(Expression expr)
    {
        switch (expr)
        {
            case LiteralExpression l:
                return GetLiteralType(l.Value);

            case IdentifierExpression id:
                var type = _scope.Lookup(id.Name);
                if (type == null)
                    throw new Exception($"Undefined variable '{id.Name}'");
                return type;

            case BinaryExpression b:
                return CheckBinary(b);

            case AssignmentExpression a:
                return CheckAssignment(a);

            case UnaryExpression u:
                return CheckUnary(u);
            
            case CallExpression c:
                return CheckCall(c);

            default:
                throw new Exception("Unsupported expression");
        }
    }
    
    private TypeSymbol CheckCall(CallExpression c)
    {
        if (c.Target is not IdentifierExpression id)
            throw new Exception("Invalid call target");

        var function = _scope.LookupFunction(id.Name);
        if (function == null)
            throw new Exception($"Undefined function '{id.Name}'");

        if (function.Parameters.Count != c.Arguments.Count)
            throw new Exception("Argument count mismatch");

        for (int i = 0; i < c.Arguments.Count; i++)
        {
            var argType = CheckExpression(c.Arguments[i]);

            if (argType != function.Parameters[i])
                throw new Exception("Argument type mismatch");
        }

        return function.ReturnType;
    }

    private TypeSymbol CheckAssignment(AssignmentExpression a)
    {
        var targetType = CheckExpression(a.Target);
        var valueType = CheckExpression(a.Value);

        if (targetType != valueType)
            throw new Exception($"Cannot assign {valueType} to {targetType}");

        return targetType;
    }

    private TypeSymbol CheckBinary(BinaryExpression b)
    {
        var left = CheckExpression(b.Left);
        var right = CheckExpression(b.Right);

        if (left != right)
            throw new Exception("Type mismatch in binary expression");

        return left;
    }

    private TypeSymbol CheckUnary(UnaryExpression u)
    {
        return CheckExpression(u.Operand);
    }

    private TypeSymbol GetLiteralType(object? value)
    {
        return value switch
        {
            int => BuiltinTypeSymbol.Int,
            float => BuiltinTypeSymbol.Float,
            bool => BuiltinTypeSymbol.Bool,
            string => BuiltinTypeSymbol.String,
            null => BuiltinTypeSymbol.Void,
            _ => throw new Exception("Unknown literal type")
        };
    }

    private TypeSymbol ResolveType(string name)
    {
        return name switch
        {
            "int" => BuiltinTypeSymbol.Int,
            "float" => BuiltinTypeSymbol.Float,
            "bool" => BuiltinTypeSymbol.Bool,
            "string" => BuiltinTypeSymbol.String,
            "void" => BuiltinTypeSymbol.Void,
            _ => throw new Exception($"Unknown type '{name}'")
        };
    }

    private void EnterScope()
    {
        _scope = new SymbolTable(_scope);
    }

    private void ExitScope()
    {
        _scope = new SymbolTable(_scope);
    }
    
    private void CheckFunction(FunctionDeclaration f)
    {
        var returnType = ResolveType(f.ReturnTypeName);

        var paramTypes = f.Parameters
            .Select(p => ResolveType(p.TypeName))
            .ToList();

        var symbol = new FunctionSymbol(
            f.Name,
            returnType,
            paramTypes);

        if (!_scope.DeclareFunction(symbol))
            throw new Exception($"Function '{f.Name}' already declared");

        EnterScope();

        for (int i = 0; i < f.Parameters.Count; i++)
        {
            _scope.Declare(f.Parameters[i].Name, paramTypes[i]);
        }

        var previousReturn = _currentReturnType;
        _currentReturnType = returnType;

        CheckStatement(f.Body);

        _currentReturnType = previousReturn;

        ExitScope();
    }
    
    private void CheckReturn(ReturnStatement r)
    {
        if (r.Expression == null)
        {
            if (_currentReturnType != BuiltinTypeSymbol.Void)
                throw new Exception("Missing return value");
            return;
        }

        var type = CheckExpression(r.Expression);

        if (type != _currentReturnType)
            throw new Exception($"Cannot return {type}, expected {_currentReturnType}");
    }
}