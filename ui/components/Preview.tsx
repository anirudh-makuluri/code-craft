"use client";
import { useEffect, useState } from "react";

export default function Preview(props: { html: string; css: string; js: string }) {
  const [srcDoc, setSrcDoc] = useState<string>("");

  useEffect(() => {
    const timeout = setTimeout(() => {
      const code = `<!DOCTYPE html><html lang="en"><head><meta charset="UTF-8"><title>CodeCraft Preview</title><style>${props.css}</style></head><body>${props.html}<script>${props.js}</script></body></html>`;
      setSrcDoc(code);
    }, 500);
    return () => clearTimeout(timeout);
  }, [props.css, props.html, props.js]);

  return (
    <iframe
      srcDoc={srcDoc}
      title='output'
      sandbox='allow-scripts'
      frameBorder='0'
      width='100%'
      height='100%'
      loading='lazy'
      className='z-[1] border-0 w-full h-full absolute top-0 left-0'
    />
  );
}
