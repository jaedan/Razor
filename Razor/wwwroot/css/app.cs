html, body
{
    font - family: 'Helvetica Neue', Helvetica, Arial, sans - serif;
}

.valid.modified:not([type = checkbox]) {
outline: 1px solid #26b050;
}

.invalid
{
outline: 1px solid red;
}

.validation - message {
color: red;
}

# blazor-error-ui {
background: lightyellow;
bottom: 0;
box - shadow: 0 - 1px 2px rgba(0, 0, 0, 0.2);
display: none;
left: 0;
padding: 0.6rem 1.25rem 0.7rem 1.25rem;
position: fixed;
width: 100 %;
z - index: 1000;
}

    #blazor-error-ui .dismiss {
        cursor: pointer;
position: absolute;
right: 0.75rem;
top: 0.5rem;
    }