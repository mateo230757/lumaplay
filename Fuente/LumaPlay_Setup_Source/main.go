//go:build windows

package main

import (
	"archive/zip"
	"bytes"
	"embed"
	"fmt"
	"io"
	"os"
	"os/exec"
	"path/filepath"
	"strings"
	"syscall"
	"unsafe"
)

// The complete LumaPlay installer payload is embedded in this executable.
//
//go:embed payload.zip
var payload embed.FS

var (
	user32      = syscall.NewLazyDLL("user32.dll")
	messageBoxW = user32.NewProc("MessageBoxW")
)

const (
	mbOK              = 0x00000000
	mbOKCancel        = 0x00000001
	mbIconInformation = 0x00000040
	mbIconError       = 0x00000010
	mbIconWarning     = 0x00000030
	mbSetForeground   = 0x00010000
	idOK              = 1
)

func message(title, body string, flags uintptr) int {
	t, _ := syscall.UTF16PtrFromString(title)
	b, _ := syscall.UTF16PtrFromString(body)
	r, _, _ := messageBoxW.Call(0, uintptr(unsafe.Pointer(b)), uintptr(unsafe.Pointer(t)), flags|mbSetForeground)
	return int(r)
}

func main() {
	if message("LumaPlay Setup", "Este asistente instalará LumaPlay en tu PC.\n\nSe abrirá una ventana de PowerShell para elegir la carpeta y los accesos directos.\n\n¿Continuar?", mbOKCancel|mbIconInformation) != idOK {
		return
	}

	root, err := os.MkdirTemp(os.TempDir(), "LumaPlay-Setup-")
	if err != nil {
		message("LumaPlay Setup", "No se pudo preparar el instalador:\n"+err.Error(), mbOK|mbIconError)
		return
	}
	defer os.RemoveAll(root)

	archive, err := payload.ReadFile("payload.zip")
	if err != nil {
		message("LumaPlay Setup", "No se pudo leer el paquete incluido.\n"+err.Error(), mbOK|mbIconError)
		return
	}
	if err := extractZip(archive, root); err != nil {
		message("LumaPlay Setup", "No se pudieron extraer los archivos:\n"+err.Error(), mbOK|mbIconError)
		return
	}

	projectDir := filepath.Join(root, "LumaPlay_Project")
	script := filepath.Join(projectDir, "Instalar-LumaPlay.ps1")
	if _, err := os.Stat(script); err != nil {
		message("LumaPlay Setup", "Falta el asistente de instalación incluido en el paquete.", mbOK|mbIconError)
		return
	}

	cmd := exec.Command("powershell.exe", "-NoProfile", "-ExecutionPolicy", "Bypass", "-File", script)
	cmd.Dir = projectDir
	cmd.SysProcAttr = &syscall.SysProcAttr{CreationFlags: 0x00000010} // CREATE_NEW_CONSOLE
	err = cmd.Run()
	if err != nil {
		message("LumaPlay Setup", "El asistente terminó con un error.\n\nComprueba que Windows tenga .NET Framework 4.8 y su compilador C# (csc.exe).\n\nDetalle: "+err.Error(), mbOK|mbIconWarning)
		return
	}
	message("LumaPlay Setup", "El asistente de instalación terminó.\n\nSi no aparecieron errores, LumaPlay ya está instalada. Puedes abrirla desde el acceso directo que elegiste.", mbOK|mbIconInformation)
}

func extractZip(data []byte, destination string) error {
	zr, err := zip.NewReader(bytes.NewReader(data), int64(len(data)))
	if err != nil {
		return err
	}
	root, err := filepath.Abs(destination)
	if err != nil {
		return err
	}
	for _, f := range zr.File {
		clean := filepath.Clean(filepath.FromSlash(f.Name))
		if clean == "." || filepath.IsAbs(clean) || clean == ".." || strings.HasPrefix(clean, ".."+string(os.PathSeparator)) {
			return fmt.Errorf("ruta no válida dentro del paquete: %s", f.Name)
		}
		target := filepath.Join(root, clean)
		if !strings.HasPrefix(target, root+string(os.PathSeparator)) && target != root {
			return fmt.Errorf("ruta fuera de la carpeta temporal: %s", f.Name)
		}
		if f.FileInfo().IsDir() {
			if err := os.MkdirAll(target, 0755); err != nil {
				return err
			}
			continue
		}
		if err := os.MkdirAll(filepath.Dir(target), 0755); err != nil {
			return err
		}
		r, err := f.Open()
		if err != nil {
			return err
		}
		mode := f.Mode().Perm()
		if mode == 0 {
			mode = 0644
		}
		out, err := os.OpenFile(target, os.O_CREATE|os.O_TRUNC|os.O_WRONLY, mode)
		if err != nil {
			r.Close()
			return err
		}
		_, copyErr := io.Copy(out, r)
		closeOutErr := out.Close()
		closeInErr := r.Close()
		if copyErr != nil {
			return copyErr
		}
		if closeOutErr != nil {
			return closeOutErr
		}
		if closeInErr != nil {
			return closeInErr
		}
	}
	return nil
}
