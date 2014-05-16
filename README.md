ParserGenerator
===============

A library for generating parsers for any language defined by an LL(*) grammar.

Uses itself to generate a parser for parsing input grammar expressions.

Example input grammar for a Lisp like language:

<pre>
PROGRAM            = EXPR
EXPR               = (EXPRESSION | '(' EXPR ')' )
EXPRESSION         = (VALUE|(MATH_EXPRESSION|LOGICAL_EXPRESSION|LAMBDA))
VALUE              = (NUMBER|VARIABLE)
MATH_EXPRESSION    = MATH_OP EXPR EXPR
LOGICAL_EXPRESSION = LOGICAL_OP EXPR EXPR EXPR EXPR
LAMBDA             = '(' 'lambda' ARGS_LIST EXPR (EXPR)+ ')'
ARGS_LIST          = '(' VARIABLE (',' VARIABLE)* ')'
REGEX:VARIABLE     = '[a-zA-Z]+'
REGEX:NUMBER       = '[0-9]+'
MATH_OP            = ('+'|'-'|'*'|'/')
LOGICAL_OP         = ('<'|'>'|'<='|'>=')
</pre>

Example parser input for the above grammar:

<pre>
(+ 1 (lambda (x,y) (+ x y) 1 (* 2 2)))
</pre>
